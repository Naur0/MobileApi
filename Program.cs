using Microsoft.EntityFrameworkCore;
using MobileApi.Data;
using MobileApi.Models;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddControllers();
builder.Services.AddHttpClient("Nominatim", client =>
{
    client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MobileApi/1.0");
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("PostDb"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

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

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
