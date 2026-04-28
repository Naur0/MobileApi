using Microsoft.EntityFrameworkCore;
using MobileApi.Data;

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

app.UseCors();

app.MapControllers();

app.Run();