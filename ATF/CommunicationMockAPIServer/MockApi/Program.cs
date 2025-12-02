using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var app = WebApplication.Create(args);

// 🧩 Optional: Simulate network latency
app.Use(async (ctx, next) => {
    await Task.Delay(300); // 300ms delay
    await next();
});

// ✅ Simple GET endpoint
app.MapGet("/users/{id}", (int id) =>
    Results.Json(new { id, name = "Mocky McMockface" })
);

// ✅ Example POST (login simulation)
app.MapPost("/login", async (HttpRequest req) => {
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(req.Body);
    var username = body?["username"] ?? "guest";
    return Results.Json(new { token = $"fake-jwt-for-{username}" });
});

// ❌ Example of error simulation
app.MapGet("/unstable", () => {
    if (Random.Shared.NextDouble() < 0.3)
        return Results.Json(new { error = "Random server failure" }, statusCode: 500);
    return Results.Json(new { ok = true });
});

// 🔒 Example with headers
app.MapGet("/secure", (HttpRequest req) =>
{
    if (!req.Headers.TryGetValue("Authorization", out var token) || token != "Bearer fake-jwt")
        return Results.Json(new { error = "Unauthorized" }, statusCode: 401);

    return Results.Json(new { data = "Protected mock data" });
});

app.Run("http://localhost:4000");
