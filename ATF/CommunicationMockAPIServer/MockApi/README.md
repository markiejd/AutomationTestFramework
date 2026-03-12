# ATF Mock API Server

A lightweight mock API server for testing automation scenarios. Runs on `http://localhost:4000/`

## 🚀 Quick Start

1. Navigate to this folder: `CommunicationMockAPIServer\MockApi`
2. Run in the terminal (or execute runme.bat for new terminal):
   dotnet run
3. The API server will start on `http://localhost:4000/`

## 📚 Built-in Endpoints

### GET /users/{id}
Returns a mock user object.

GET http://localhost:4000/users/123
**Response:**
{"id":123,"name":"Mocky McMockface"}


### POST /login
Simulates a login endpoint that returns a fake JWT token.
```bash
POST http://localhost:4000/login
Content-Type: application/json

{"username":"testuser"}
```
**Response:**
```json
{"token":"fake-jwt-for-testuser"}
```

### GET /unstable
Randomly returns success or error (30% failure rate) for testing error handling.
```bash
GET http://localhost:4000/unstable
```

### GET /secure
Requires Authorization header for testing authenticated endpoints.
```bash
GET http://localhost:4000/secure
Authorization: Bearer fake-jwt
```

## 🛠️ Creating Custom Endpoints

All endpoints are defined in `Program.cs`. Here's how to add your own:

### Simple GET Endpoint
Add this code to `Program.cs` before `app.Run()`:
```csharp
app.MapGet("/products/{id}", (int id) =>
    Results.Json(new { id, name = "Product " + id, price = 99.99 })
);
```

### POST Endpoint with Body
```csharp
app.MapPost("/orders", async (HttpRequest req) => {
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(req.Body);
    return Results.Json(new { 
        orderId = Guid.NewGuid().ToString(), 
        status = "created",
        items = body?["items"]
    });
});
```

### PUT Endpoint
```csharp
app.MapPut("/users/{id}", async (int id, HttpRequest req) => {
    var body = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(req.Body);
    return Results.Json(new { 
        id, 
        name = body?["name"],
        updated = true 
    });
});
```

### DELETE Endpoint
```csharp
app.MapDelete("/users/{id}", (int id) =>
    Results.Json(new { id, deleted = true })
);
```

### Custom Status Codes
```csharp
app.MapGet("/notfound", () =>
    Results.Json(new { error = "Resource not found" }, statusCode: 404)
);
```

### Checking Headers
```csharp
app.MapGet("/api-key-check", (HttpRequest req) => {
    if (!req.Headers.TryGetValue("X-API-Key", out var apiKey) || apiKey != "secret123")
        return Results.Json(new { error = "Invalid API key" }, statusCode: 403);
    
    return Results.Json(new { success = true });
});
```

## 💡 Tips

- **Network Latency**: The server includes a 300ms delay to simulate real network conditions. Adjust in `Program.cs`
- **Restart**: After adding new endpoints, stop (Ctrl+C) and restart the server
- **Testing**: Use tools like Postman, curl, or your automation framework to test endpoints
- **Port**: Default is 4000. Change in `app.Run("http://localhost:4000")` if needed

## 📝 Example Usage in Tests

```csharp
// Example with HttpClient
var client = new HttpClient { BaseAddress = new Uri("http://localhost:4000") };
var response = await client.GetAsync("/users/123");
var user = await response.Content.ReadFromJsonAsync<dynamic>();
```