using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        Description = "A mock API server for automation testing. Use Admin endpoints to switch scenarios, inject faults, and inspect recorded requests. Default behavior is scenario='happy' with fault injection disabled. For JWT authentication examples, first call /examples/auth/login, copy only the raw token value from the response, then click Authorize in Swagger and paste only that token. Swagger will add the Bearer prefix for you."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste only the raw JWT token here, without the word Bearer. Example: eyJhbGciOi... Swagger will automatically send Authorization: Bearer {token}."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            new List<string>()
        }
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

// 🆕 New typed example endpoints for Swagger/testing without changing existing routes
app.MapPost("/examples/login", (ExampleLoginRequest request) =>
{
    var username = string.IsNullOrWhiteSpace(request.Username) ? "guest" : request.Username.Trim();
    return Results.Ok(new ExampleLoginResponse(
        Token: $"fake-jwt-for-{username}",
        Username: username,
        ExpiresUtc: DateTime.UtcNow.AddHours(1)));
})
.WithName("ExampleLogin")
.WithSummary("Typed login example")
.WithDescription("Testing use: demonstrates a typed login request/response in Swagger without modifying the original /login endpoint.\nSend JSON like { \"username\": \"alice\" }.\nExpected 200 returns { token, username, expiresUtc }.")
.WithTags("Examples")
.Accepts<ExampleLoginRequest>("application/json")
.Produces<ExampleLoginResponse>(200);

app.MapPost("/examples/orders", (ExampleCreateOrderRequest request) =>
{
    var orderId = $"ORD-{Random.Shared.Next(1000, 9999)}";
    var total = request.Items.Sum(item => item.Quantity * item.UnitPrice);

    return Results.Created($"/examples/orders/{orderId}", new ExampleCreateOrderResponse(
        OrderId: orderId,
        CustomerId: request.CustomerId,
        Status: "created",
        Currency: request.Currency,
        Total: total,
        ItemCount: request.Items.Count));
})
.WithName("CreateExampleOrder")
.WithSummary("Create a typed mock order")
.WithDescription("Testing use: demonstrates a POST endpoint with a richer request body and a 201 Created response.\nExpected 201 returns { orderId, customerId, status, currency, total, itemCount }.")
.WithTags("Examples")
.Accepts<ExampleCreateOrderRequest>("application/json")
.Produces<ExampleCreateOrderResponse>(201);

app.MapGet("/examples/orders/{orderId}", (string orderId) =>
    Results.Ok(new ExampleOrderResponse(
        OrderId: orderId,
        CustomerId: "CUST-1001",
        Status: "processing",
        Currency: "GBP",
        Total: 149.97m,
        Items:
        [
            new ExampleOrderItemResponse("SKU-RED-01", 1, 49.99m),
            new ExampleOrderItemResponse("SKU-BLU-02", 2, 49.99m)
        ])))
.WithName("GetExampleOrder")
.WithSummary("Get a typed mock order")
.WithDescription("Testing use: demonstrates a typed GET response with nested data.\nExpected 200 returns a single order with item details.")
.WithTags("Examples")
.Produces<ExampleOrderResponse>(200);

app.MapPost("/examples/customers/validate", (ExampleCustomerValidationRequest request) =>
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(request.FirstName))
        errors["firstName"] = ["First name is required."];

    if (string.IsNullOrWhiteSpace(request.LastName))
        errors["lastName"] = ["Last name is required."];

    if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        errors["email"] = ["A valid email address is required."];

    if (errors.Count > 0)
    {
        return Results.BadRequest(new ExampleValidationErrorResponse(
            Message: "Validation failed",
            Errors: errors));
    }

    return Results.Ok(new ExampleCustomerValidationSuccessResponse(
        Message: "Customer payload is valid",
        CustomerReference: $"CUS-{Random.Shared.Next(10000, 99999)}"));
})
.WithName("ValidateExampleCustomer")
.WithSummary("Validate a customer payload")
.WithDescription("Testing use: demonstrates success and validation-error responses in Swagger.\nExpected 200 when FirstName, LastName, and Email are valid.\nExpected 400 returns { message, errors } when required fields are missing or malformed.")
.WithTags("Examples")
.Accepts<ExampleCustomerValidationRequest>("application/json")
.Produces<ExampleCustomerValidationSuccessResponse>(200)
.Produces<ExampleValidationErrorResponse>(400);

app.MapGet("/examples/rows", (int rows, int? skip) =>
{
    var normalizedRows = Math.Clamp(rows, 1, 1000);
    var normalizedSkip = Math.Max(0, skip ?? 0);

    var items = Enumerable.Range(normalizedSkip + 1, normalizedRows)
        .Select(rowNumber => new ExampleRowItem(
            RowNumber: rowNumber,
            Reference: $"ROW-{rowNumber:000000}",
            Description: $"Mock row {rowNumber}"))
        .ToList();

    return Results.Ok(new ExamplePagedRowsResponse(
        Skip: normalizedSkip,
        Rows: normalizedRows,
        Returned: items.Count,
        Items: items));
})
.WithName("GetExampleRows")
.WithSummary("Return a requested number of mock rows")
.WithDescription("Testing use: simulate paged result sets. Provide rows and optional skip as query string values. Example: /examples/rows?rows=3&skip=100 returns 3 rows starting at row 101. Expected 200 returns { skip, rows, returned, items[] }.")
.WithTags("Examples")
.Produces<ExamplePagedRowsResponse>(200);

app.MapGet("/examples/rows/paged", (int rows, int? skip, int? totalRows) =>
{
    var normalizedRows = Math.Clamp(rows, 1, 1000);
    var normalizedSkip = Math.Max(0, skip ?? 0);
    var normalizedTotalRows = Math.Max(normalizedSkip, totalRows ?? 500);
    var returned = Math.Max(0, Math.Min(normalizedRows, normalizedTotalRows - normalizedSkip));

    var items = Enumerable.Range(normalizedSkip + 1, returned)
        .Select(rowNumber => new ExampleRowItem(
            RowNumber: rowNumber,
            Reference: $"ROW-{rowNumber:000000}",
            Description: $"Mock row {rowNumber}"))
        .ToList();

    var hasMore = normalizedSkip + returned < normalizedTotalRows;
    int? nextSkip = hasMore ? normalizedSkip + returned : null;

    return Results.Ok(new ExamplePagedRowsMetadataResponse(
        Skip: normalizedSkip,
        Rows: normalizedRows,
        Returned: returned,
        TotalRows: normalizedTotalRows,
        HasMore: hasMore,
        NextSkip: nextSkip,
        Items: items));
})
.WithName("GetExampleRowsPaged")
.WithSummary("Return mock rows with paging metadata")
.WithDescription("Testing use: simulate paged result sets when the caller also needs totalRows, hasMore, and nextSkip. Example: /examples/rows/paged?rows=3&skip=100&totalRows=105 returns rows 101-103 with hasMore=true and nextSkip=103.")
.WithTags("Examples")
.Produces<ExamplePagedRowsMetadataResponse>(200);

app.MapGet("/examples/rows/by-page", (int pageSize, int? pageNumber, int? totalRows) =>
{
    var normalizedPageSize = Math.Clamp(pageSize, 1, 1000);
    var normalizedPageNumber = Math.Max(1, pageNumber ?? 1);
    var normalizedTotalRows = Math.Max(0, totalRows ?? 500);
    var skip = (normalizedPageNumber - 1) * normalizedPageSize;
    var returned = skip >= normalizedTotalRows
        ? 0
        : Math.Min(normalizedPageSize, normalizedTotalRows - skip);

    var items = returned == 0
        ? []
        : Enumerable.Range(skip + 1, returned)
            .Select(rowNumber => new ExampleRowItem(
                RowNumber: rowNumber,
                Reference: $"ROW-{rowNumber:000000}",
                Description: $"Mock row {rowNumber}"))
            .ToList();

    var totalPages = normalizedTotalRows == 0
        ? 0
        : (int)Math.Ceiling(normalizedTotalRows / (double)normalizedPageSize);
    var hasMore = normalizedPageNumber < totalPages;
    int? nextPageNumber = hasMore ? normalizedPageNumber + 1 : null;

    return Results.Ok(new ExamplePageNumberRowsResponse(
        PageNumber: normalizedPageNumber,
        PageSize: normalizedPageSize,
        Skip: skip,
        Returned: returned,
        TotalRows: normalizedTotalRows,
        TotalPages: totalPages,
        HasMore: hasMore,
        NextPageNumber: nextPageNumber,
        Items: items));
})
.WithName("GetExampleRowsByPage")
.WithSummary("Return mock rows by page number")
.WithDescription("Testing use: simulate APIs that page by pageNumber and pageSize instead of skip and rows. Example: /examples/rows/by-page?pageSize=25&pageNumber=2&totalRows=60 returns rows 26-50 and shows page metadata.")
.WithTags("Examples")
.Produces<ExamplePageNumberRowsResponse>(200);

// 🔐 AUTHENTICATION EXAMPLES
app.MapPost("/examples/auth/login", (ExampleJwtLoginRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Username))
    {
        return Results.BadRequest(new ExampleAuthErrorResponse("Username is required."));
    }

    var username = request.Username.Trim();
    var roles = request.Roles is { Length: > 0 }
        ? request.Roles.Where(role => !string.IsNullOrWhiteSpace(role)).Select(role => role.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
        : ["reader"];

    var expiresUtc = DateTime.UtcNow.AddMinutes(30);
    var token = CreateJwtToken(username, roles, expiresUtc);

    return Results.Ok(new ExampleJwtLoginResponse(
        Token: token,
        TokenType: "Bearer",
        ExpiresUtc: expiresUtc,
        Username: username,
        Roles: roles));
})
.WithName("ExampleJwtLogin")
.WithSummary("Issue a JWT token")
.WithDescription("Testing use: call this endpoint first to get a real JWT bearer token for the authentication examples. Send JSON like { \"username\": \"alice\", \"roles\": [\"reader\"] }. Expected 200 returns { token, tokenType, expiresUtc, username, roles }. In Swagger, copy only the token value, click Authorize, and paste only the token. Do not add Bearer yourself.")
.WithTags("AUTHENTICATION EXAMPLES")
.Accepts<ExampleJwtLoginRequest>("application/json")
.Produces<ExampleJwtLoginResponse>(200)
.Produces<ExampleAuthErrorResponse>(400);

app.MapGet("/examples/auth/profile", ([FromHeader(Name = "Authorization")] string? authorization) =>
{
    if (!TryValidateJwtToken(authorization, out var principal, out var errorMessage))
    {
        return Results.Json(new ExampleAuthErrorResponse(errorMessage), statusCode: 401);
    }

    var username = principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? principal?.FindFirst(ClaimTypes.Name)?.Value
        ?? "unknown";
    var roles = principal?.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray() ?? [];

    return Results.Ok(new ExampleJwtProtectedResponse(
        Message: "JWT token accepted.",
        Username: username,
        Roles: roles,
        AuthenticationType: principal?.Identity?.AuthenticationType ?? "Bearer"));
})
.WithName("ExampleJwtProfile")
.WithSummary("Use a JWT token to access a protected endpoint")
.WithDescription("Testing use: call /examples/auth/login first, copy the returned token, then click Authorize in Swagger and paste only the token value. Swagger will send Authorization: Bearer {token} for this endpoint. Expected 200 only when the JWT is valid and signed by this mock server. Expected 401 when the token is missing, malformed, expired, or signed with a different key.")
.WithTags("AUTHENTICATION EXAMPLES")
.Produces<ExampleJwtProtectedResponse>(200)
.Produces<ExampleAuthErrorResponse>(401);

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

static string CreateJwtToken(string username, string[] roles, DateTime expiresUtc)
{
    var credentials = new SigningCredentials(GetJwtSigningKey(), SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, username),
        new(ClaimTypes.Name, username),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var token = new JwtSecurityToken(
        issuer: GetJwtIssuer(),
        audience: GetJwtAudience(),
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: expiresUtc,
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

static bool TryValidateJwtToken(string? authorization, out ClaimsPrincipal? principal, out string errorMessage)
{
    principal = null;

    if (string.IsNullOrWhiteSpace(authorization))
    {
        errorMessage = "Authorization header is required. Use 'Bearer {token}'.";
        return false;
    }

    const string bearerPrefix = "Bearer ";
    if (!authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
    {
        errorMessage = "Authorization header must start with 'Bearer '.";
        return false;
    }

    var token = authorization[bearerPrefix.Length..].Trim();
    if (string.IsNullOrWhiteSpace(token))
    {
        errorMessage = "Bearer token was empty.";
        return false;
    }

    var validationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = GetJwtIssuer(),
        ValidateAudience = true,
        ValidAudience = GetJwtAudience(),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = GetJwtSigningKey(),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    try
    {
        principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
        errorMessage = string.Empty;
        return true;
    }
    catch (Exception ex)
    {
        errorMessage = $"Invalid token: {ex.Message}";
        return false;
    }
}

static SymmetricSecurityKey GetJwtSigningKey() =>
    new(System.Text.Encoding.UTF8.GetBytes("ATF-MockApi-JWT-Signing-Key-2026-Example-Only"));

static string GetJwtIssuer() => "ATF.MockApi";

static string GetJwtAudience() => "ATF.MockApi.Client";

sealed record ExampleLoginRequest(string? Username);
sealed record ExampleLoginResponse(string Token, string Username, DateTime ExpiresUtc);
sealed record ExampleCreateOrderRequest(string CustomerId, string Currency, List<ExampleCreateOrderItemRequest> Items);
sealed record ExampleCreateOrderItemRequest(string Sku, int Quantity, decimal UnitPrice);
sealed record ExampleCreateOrderResponse(string OrderId, string CustomerId, string Status, string Currency, decimal Total, int ItemCount);
sealed record ExampleOrderResponse(string OrderId, string CustomerId, string Status, string Currency, decimal Total, List<ExampleOrderItemResponse> Items);
sealed record ExampleOrderItemResponse(string Sku, int Quantity, decimal UnitPrice);
sealed record ExampleCustomerValidationRequest(string? FirstName, string? LastName, string? Email);
sealed record ExampleCustomerValidationSuccessResponse(string Message, string CustomerReference);
sealed record ExampleValidationErrorResponse(string Message, Dictionary<string, string[]> Errors);
sealed record ExamplePagedRowsResponse(int Skip, int Rows, int Returned, List<ExampleRowItem> Items);
sealed record ExamplePagedRowsMetadataResponse(int Skip, int Rows, int Returned, int TotalRows, bool HasMore, int? NextSkip, List<ExampleRowItem> Items);
sealed record ExamplePageNumberRowsResponse(int PageNumber, int PageSize, int Skip, int Returned, int TotalRows, int TotalPages, bool HasMore, int? NextPageNumber, List<ExampleRowItem> Items);
sealed record ExampleRowItem(int RowNumber, string Reference, string Description);
sealed record ExampleJwtLoginRequest(string Username, string[]? Roles);
sealed record ExampleJwtLoginResponse(string Token, string TokenType, DateTime ExpiresUtc, string Username, string[] Roles);
sealed record ExampleJwtProtectedResponse(string Message, string Username, string[] Roles, string AuthenticationType);
sealed record ExampleAuthErrorResponse(string Error);

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
