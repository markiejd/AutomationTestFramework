using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var mockState = new MockServerState();
var requestJournal = new RequestJournal(maxEntries: 2000);

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { 
        Title = "ATF Mock API Server", 
        Version = "v1",
        Description = "A mock API server for automation testing. Use Admin endpoints to switch scenarios, inject faults, and inspect recorded requests. Default behavior is scenario='happy' with fault injection disabled."
    });
});

var app = builder.Build();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ATF Mock API v1");
    options.RoutePrefix = string.Empty; // Set Swagger UI at root
});

app.Use(async (ctx, next) =>
{
    var path = ctx.Request.Path.Value ?? string.Empty;
    var isAdminOrSwagger =
        path.StartsWith("/__admin", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);

    if (isAdminOrSwagger)
    {
        await next();
        return;
    }

    ctx.Request.EnableBuffering();
    var requestBody = await ReadRequestBodyAsync(ctx.Request);
    ctx.Request.Body.Position = 0;

    if (mockState.ActiveScenario.Equals("always-500", StringComparison.OrdinalIgnoreCase))
    {
        await Results.Json(new { error = "Scenario forced error", scenario = mockState.ActiveScenario }, statusCode: 500).ExecuteAsync(ctx);
        requestJournal.Add(BuildRecord(ctx, requestBody, "{\"error\":\"Scenario forced error\"}", 500, 0));
        return;
    }

    if (mockState.ActiveScenario.Equals("timeout", StringComparison.OrdinalIgnoreCase))
    {
        await Task.Delay(mockState.TimeoutMs);
        await Results.Json(new { error = "Scenario timeout", scenario = mockState.ActiveScenario }, statusCode: 504).ExecuteAsync(ctx);
        requestJournal.Add(BuildRecord(ctx, requestBody, "{\"error\":\"Scenario timeout\"}", 504, mockState.TimeoutMs));
        return;
    }

    if (mockState.ActiveScenario.Equals("unauthorized", StringComparison.OrdinalIgnoreCase))
    {
        await Results.Json(new { error = "Scenario unauthorized", scenario = mockState.ActiveScenario }, statusCode: 401).ExecuteAsync(ctx);
        requestJournal.Add(BuildRecord(ctx, requestBody, "{\"error\":\"Scenario unauthorized\"}", 401, 0));
        return;
    }

    if (mockState.Faults.Enabled)
    {
        var jitter = mockState.Faults.JitterMs > 0
            ? Random.Shared.Next(-mockState.Faults.JitterMs, mockState.Faults.JitterMs + 1)
            : 0;
        var delay = Math.Max(0, mockState.Faults.BaseLatencyMs + jitter);
        if (delay > 0)
        {
            await Task.Delay(delay);
        }

        if (mockState.Faults.AbortConnection)
        {
            ctx.Abort();
            return;
        }

        if (mockState.Faults.ForcedStatusCode.HasValue)
        {
            var statusCode = mockState.Faults.ForcedStatusCode.Value;
            await Results.Json(new
            {
                error = "Fault injection forced status code",
                statusCode,
                scenario = mockState.ActiveScenario
            }, statusCode: statusCode).ExecuteAsync(ctx);
            requestJournal.Add(BuildRecord(ctx, requestBody, $"{{\"error\":\"Forced {statusCode}\"}}", statusCode, delay));
            return;
        }

        if (mockState.Faults.ErrorRate > 0 && Random.Shared.NextDouble() < mockState.Faults.ErrorRate)
        {
            await Results.Json(new
            {
                error = "Fault injection random failure",
                scenario = mockState.ActiveScenario
            }, statusCode: 500).ExecuteAsync(ctx);
            requestJournal.Add(BuildRecord(ctx, requestBody, "{\"error\":\"Fault injection random failure\"}", 500, delay));
            return;
        }
    }

    var stopwatch = Stopwatch.StartNew();
    var originalBody = ctx.Response.Body;
    await using var captureStream = new MemoryStream();
    ctx.Response.Body = captureStream;

    try
    {
        await next();

        if (mockState.ActiveScenario.Equals("malformed-json", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "application/json";
            captureStream.SetLength(0);
            await using var writer = new StreamWriter(captureStream, leaveOpen: true);
            await writer.WriteAsync("{\"broken\": ");
            await writer.FlushAsync();
        }
    }
    finally
    {
        stopwatch.Stop();
        captureStream.Position = 0;
        var responseBody = await new StreamReader(captureStream).ReadToEndAsync();
        captureStream.Position = 0;
        await captureStream.CopyToAsync(originalBody);
        ctx.Response.Body = originalBody;

        requestJournal.Add(BuildRecord(ctx, requestBody, responseBody, ctx.Response.StatusCode, stopwatch.ElapsedMilliseconds));
    }
});

// 🧪 Admin endpoints for test control
app.MapGet("/__admin/scenarios", () =>
    Results.Json(new
    {
        active = mockState.ActiveScenario,
        available = mockState.AvailableScenarios
    }))
.WithName("GetScenarios")
.WithSummary("List active and available scenarios")
.WithDescription("Testing use: verify scenario discovery before a test starts.\nExpected 200 response includes { active, available[] }.")
.WithTags("Admin");

app.MapPost("/__admin/scenarios/{name}/activate", (string name) =>
{
    if (!mockState.TryActivateScenario(name))
    {
        return Results.NotFound(new
        {
            error = "Unknown scenario",
            name,
            available = mockState.AvailableScenarios
        });
    }

    return Results.Ok(new
    {
        message = "Scenario activated",
        active = mockState.ActiveScenario
    });
})
.WithName("ActivateScenario")
.WithSummary("Activate a scenario")
.WithDescription("Testing use: force all business endpoints into one behavior profile.\nExpected 200: { message, active }.\nExpected 404: unknown scenario name.")
.WithTags("Admin")
.Produces(200)
.Produces(404);

app.MapGet("/__admin/faults", () => Results.Json(mockState.Faults))
.WithName("GetFaultInjection")
.WithSummary("Get current fault injection settings")
.WithDescription("Testing use: assert fault settings before sending test traffic.\nExpected 200 with current fault state (Enabled, BaseLatencyMs, JitterMs, ErrorRate, ForcedStatusCode, AbortConnection).")
.WithTags("Admin");

app.MapPut("/__admin/faults", (FaultInjectionUpdate update) =>
{
    mockState.Faults.Apply(update);
    return Results.Ok(new
    {
        message = "Fault settings updated",
        faults = mockState.Faults
    });
})
.WithName("UpdateFaultInjection")
.WithSummary("Update fault injection settings")
.WithDescription("Testing use: inject latency, random failures, forced status code, or aborted connections.\nExpected 200 returns updated settings.")
.WithTags("Admin")
.Accepts<FaultInjectionUpdate>("application/json")
.Produces(200);

app.MapPost("/__admin/reset", () =>
{
    mockState.Reset();
    requestJournal.Clear();

    return Results.Ok(new
    {
        message = "Mock server state reset",
        active = mockState.ActiveScenario,
        faults = mockState.Faults
    });
})
.WithName("ResetMockServer")
.WithSummary("Reset mock state")
.WithDescription("Testing use: clean slate between tests.\nExpected 200 resets scenario='happy', default faults, and clears recorded requests.")
.WithTags("Admin")
.Produces(200);

app.MapGet("/__admin/requests", (string? path, string? method, int? limit) =>
{
    var results = requestJournal.Query(path, method, limit ?? 100);
    return Results.Json(new
    {
        count = results.Count,
        items = results
    });
})
.WithName("GetRecordedRequests")
.WithSummary("Query recorded requests")
.WithDescription("Testing use: verify outbound calls made by the system under test. Optional filters: path, method, limit.\nExpected 200 includes { count, items[] } sorted newest first.")
.WithTags("Admin");

app.MapDelete("/__admin/requests", () =>
{
    requestJournal.Clear();
    return Results.Ok(new { message = "Recorded requests cleared" });
})
.WithName("ClearRecordedRequests")
.WithSummary("Clear recorded requests")
.WithDescription("Testing use: clear journal before a scenario/assertion phase.\nExpected 200: { message }.")
.WithTags("Admin")
.Produces(200);

// ✅ Simple GET endpoint
app.MapGet("/users/{id}", (int id) =>
    Results.Json(new { id, name = "Mocky McMockface" })
)
.WithName("GetUser")
.WithSummary("Get a mock user")
.WithDescription("Testing use: stable happy-path read endpoint.\nExpected 200: { id, name }.\nScenario impact: always-500 => 500, timeout => 504, unauthorized => 401, malformed-json => invalid JSON body.")
.WithTags("Dynamic URLs")
.Produces(200);

// ✅ Example POST (login simulation)
app.MapPost("/login", async (HttpRequest req) => {
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(req.Body);
    var username = body is not null && body.TryGetValue("username", out var value) ? value : "guest";
    return Results.Json(new { token = $"fake-jwt-for-{username}" });
})
.WithName("Login")
.WithSummary("Simulate login")
.WithDescription("Testing use: authenticate test flows without real identity provider.\nRequest body example: { \"username\": \"alice\" }.\nExpected 200: { token: \"fake-jwt-for-{username}\" }. Missing username defaults to guest.")
.WithTags("Authentication")
.Accepts<Dictionary<string, string>>("application/json")
.Produces(200);

// ❌ Example of error simulation
app.MapGet("/unstable", () => {
    if (mockState.ActiveScenario.Equals("always-500", StringComparison.OrdinalIgnoreCase))
        return Results.Json(new { error = "Scenario forced server failure" }, statusCode: 500);

    if (Random.Shared.NextDouble() < 0.3)
        return Results.Json(new { error = "Random server failure" }, statusCode: 500);
    return Results.Json(new { ok = true });
})
.WithName("UnstableEndpoint")
.WithSummary("Unstable endpoint for resiliency tests")
.WithDescription("Testing use: retry/circuit-breaker validation.\nExpected 200: { ok: true } most of the time.\nExpected 500: random failure (~30%) or always when scenario='always-500'.")
.WithTags("Error Handling")
.Produces(200)
.Produces(500);

// 🔒 Example with headers
app.MapGet("/secure", (HttpRequest req) =>
{
    if (!req.Headers.TryGetValue("Authorization", out var token) || token != "Bearer fake-jwt")
        return Results.Json(new { error = "Unauthorized" }, statusCode: 401);

    return Results.Json(new { data = "Protected mock data" });
})
.WithName("SecureEndpoint")
.WithSummary("Protected endpoint")
.WithDescription("Testing use: auth header handling.\nExpected 200 with Authorization: Bearer fake-jwt.\nExpected 401 when header is missing/invalid or scenario='unauthorized'.")
.WithTags("Authentication")
.Produces(200)
.Produces(401);

app.Run("http://localhost:4000");

static async Task<string> ReadRequestBodyAsync(HttpRequest request)
{
    if (request.ContentLength is null or 0)
    {
        return string.Empty;
    }

    using var reader = new StreamReader(request.Body, leaveOpen: true);
    var content = await reader.ReadToEndAsync();
    return Truncate(content, 4000);
}

static RecordedRequest BuildRecord(HttpContext ctx, string requestBody, string responseBody, int statusCode, long durationMs)
{
    var headers = ctx.Request.Headers
        .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
        .ToDictionary(h => h.Key, h => h.Value.ToString());

    return new RecordedRequest(
        Id: Guid.NewGuid(),
        TimestampUtc: DateTime.UtcNow,
        Method: ctx.Request.Method,
        Path: ctx.Request.Path.Value ?? string.Empty,
        QueryString: ctx.Request.QueryString.Value ?? string.Empty,
        Headers: headers,
        RequestBody: Truncate(requestBody, 4000),
        ResponseStatusCode: statusCode,
        ResponseBody: Truncate(responseBody, 4000),
        DurationMs: durationMs);
}

static string Truncate(string value, int maxLength) =>
    value.Length <= maxLength ? value : value[..maxLength];

sealed class MockServerState
{
    private static readonly string[] _availableScenarios =
    [
        "happy",
        "always-500",
        "timeout",
        "malformed-json",
        "unauthorized"
    ];

    public string ActiveScenario { get; private set; } = "happy";
    public int TimeoutMs { get; private set; } = 7000;
    public FaultInjectionSettings Faults { get; } = new();
    public IReadOnlyCollection<string> AvailableScenarios => _availableScenarios;

    public bool TryActivateScenario(string name)
    {
        if (!_availableScenarios.Contains(name, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        ActiveScenario = _availableScenarios.First(s => s.Equals(name, StringComparison.OrdinalIgnoreCase));
        return true;
    }

    public void Reset()
    {
        ActiveScenario = "happy";
        TimeoutMs = 7000;
        Faults.Reset();
    }
}

sealed class FaultInjectionSettings
{
    public bool Enabled { get; set; } = false;
    public int BaseLatencyMs { get; set; } = 300;
    public int JitterMs { get; set; } = 0;
    public double ErrorRate { get; set; } = 0.0;
    public int? ForcedStatusCode { get; set; }
    public bool AbortConnection { get; set; } = false;

    public void Apply(FaultInjectionUpdate update)
    {
        if (update.Enabled.HasValue) Enabled = update.Enabled.Value;
        if (update.BaseLatencyMs.HasValue) BaseLatencyMs = Math.Max(0, update.BaseLatencyMs.Value);
        if (update.JitterMs.HasValue) JitterMs = Math.Max(0, update.JitterMs.Value);
        if (update.ErrorRate.HasValue) ErrorRate = Math.Clamp(update.ErrorRate.Value, 0, 1);
        if (update.ForcedStatusCode.HasValue) ForcedStatusCode = update.ForcedStatusCode;
        if (update.ClearForcedStatusCode == true) ForcedStatusCode = null;
        if (update.AbortConnection.HasValue) AbortConnection = update.AbortConnection.Value;
    }

    public void Reset()
    {
        Enabled = false;
        BaseLatencyMs = 300;
        JitterMs = 0;
        ErrorRate = 0.0;
        ForcedStatusCode = null;
        AbortConnection = false;
    }
}

sealed class FaultInjectionUpdate
{
    public bool? Enabled { get; init; }
    public int? BaseLatencyMs { get; init; }
    public int? JitterMs { get; init; }
    public double? ErrorRate { get; init; }
    public int? ForcedStatusCode { get; init; }
    public bool? ClearForcedStatusCode { get; init; }
    public bool? AbortConnection { get; init; }
}

sealed record RecordedRequest(
    Guid Id,
    DateTime TimestampUtc,
    string Method,
    string Path,
    string QueryString,
    Dictionary<string, string> Headers,
    string RequestBody,
    int ResponseStatusCode,
    string ResponseBody,
    long DurationMs);

sealed class RequestJournal
{
    private readonly int _maxEntries;
    private readonly List<RecordedRequest> _items = [];
    private readonly object _sync = new();

    public RequestJournal(int maxEntries)
    {
        _maxEntries = Math.Max(1, maxEntries);
    }

    public void Add(RecordedRequest item)
    {
        lock (_sync)
        {
            _items.Add(item);
            if (_items.Count > _maxEntries)
            {
                _items.RemoveRange(0, _items.Count - _maxEntries);
            }
        }
    }

    public IReadOnlyList<RecordedRequest> Query(string? path, string? method, int limit)
    {
        lock (_sync)
        {
            path = string.IsNullOrWhiteSpace(path) ? null : path.Trim();
            method = string.IsNullOrWhiteSpace(method) ? null : method.Trim();

            IEnumerable<RecordedRequest> query = _items;

            if (!string.IsNullOrWhiteSpace(path))
            {
                query = query.Where(x => x.Path.Contains(path, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(method))
            {
                query = query.Where(x => x.Method.Equals(method, StringComparison.OrdinalIgnoreCase));
            }

            return query
                .OrderByDescending(x => x.TimestampUtc)
                .Take(Math.Clamp(limit, 1, 500))
                .ToList();
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _items.Clear();
        }
    }
}
