using Microsoft.EntityFrameworkCore;
using MobileApi.Data;
using MobileApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ IMPORTANT: Render PORT binding (must be BEFORE Build)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// -------------------- SERVICES --------------------

// Controllers
builder.Services.AddControllers();

// InMemory DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("PostDb"));

// CORS (for MAUI / frontend access)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// -------------------- APP BUILD --------------------

var app = builder.Build();

// -------------------- SEED DATA --------------------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Posts.Any())
    {
        db.Posts.AddRange(
            new Post { Title = "Welcome", Content = "Sample post data is available." },
            new Post { Title = "Render-ready", Content = "This API is working on Render." }
        );

        db.SaveChanges();
    }
}

// -------------------- MIDDLEWARE --------------------

app.UseCors();

app.MapControllers();

// Test route (for browser check)
app.MapGet("/", () =>
    Results.Ok(new
    {
        status = "running",
        message = "MobileApi is live 🚀"
    })
);

// -------------------- RUN APP --------------------

app.Run();