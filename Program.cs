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
    options.UseSqlite("Data Source=mobileapi.db"));

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
    db.Database.EnsureCreated();

    if (!db.LostItemReports.Any())
    {
        db.LostItemReports.AddRange(
            new LostItemReport
            {
                ItemName = "Black Backpack",
                Description = "Laptop bag with a silver zipper and school notes inside.",
                Category = "Bag",
                LostDate = DateTime.UtcNow.Date.AddDays(-1),
                LastSeenLocation = "Dhoby Ghaut MRT Station",
                ContactName = "Avery",
                ContactMethod = "avery@example.com",
                Status = "Open"
            },
            new LostItemReport
            {
                ItemName = "Blue Wallet",
                Description = "Folded wallet with ID cards and a transit pass.",
                Category = "Wallet",
                LostDate = DateTime.UtcNow.Date.AddDays(-2),
                LastSeenLocation = "Bugis Junction",
                ContactName = "Jordan",
                ContactMethod = "+65 8123 4567",
                Status = "In Review"
            }
        );

        db.SaveChanges();
    }
}

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
