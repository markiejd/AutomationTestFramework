using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { 
        Title = "ATF Mock API Server", 
        Version = "v1",
        Description = "A lightweight mock API server for testing automation scenarios"
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

// 🧩 Optional: Simulate network latency
app.Use(async (ctx, next) => {
    await Task.Delay(300); // 300ms delay
    await next();
});

// ✅ Simple GET endpoint
app.MapGet("/users/{id}", (int id) =>
    Results.Json(new { id, name = "Mocky McMockface" })
)
.WithName("GetUser")
.WithDescription("Returns a mock user by ID")
.WithTags("Dynamic URLs")
.Produces(200);

// ✅ Example POST (login simulation)
app.MapPost("/login", async (HttpRequest req) => {
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(req.Body);
    var username = body?["username"] ?? "guest";
    return Results.Json(new { token = $"fake-jwt-for-{username}" });
})
.WithName("Login")
.WithDescription("Simulates login and returns a fake JWT token")
.WithTags("Authentication")
.Produces(200);

// ❌ Example of error simulation
app.MapGet("/unstable", () => {
    if (Random.Shared.NextDouble() < 0.3)
        return Results.Json(new { error = "Random server failure" }, statusCode: 500);
    return Results.Json(new { ok = true });
})
.WithName("UnstableEndpoint")
.WithDescription("Randomly returns success or error (30% failure rate) for testing error handling")
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
.WithDescription("Requires Authorization header with 'Bearer fake-jwt' token")
.WithTags("Authentication")
.Produces(200)
.Produces(401);

app.Run("http://localhost:4000");
