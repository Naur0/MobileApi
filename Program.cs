using Microsoft.EntityFrameworkCore;
using MobileApi.Data;
using MobileApi.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// ✅ Controllers
builder.Services.AddControllers();

// ✅ InMemory DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("PostDb"));

// ✅ CORS (IMPORTANT for MAUI)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// ✅ Seed sample data for initial GET responses
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Posts.Any())
    {
        db.Posts.AddRange(
            new Post { Title = "Welcome", Content = "Sample post data is available." },
            new Post { Title = "Render-ready", Content = "This .NET API is configured for Render." });
        db.SaveChanges();
    }
}

app.UseCors();

app.MapControllers();
app.MapGet("/", () => Results.Ok(new { status = "running", message = "MobileApi is ready." }));

// ✅ PORT CONFIG (IMPORTANT for deployment like Render / Railway / etc.)
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");