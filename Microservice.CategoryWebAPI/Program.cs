using Carter;
using Microservice.CategoryWebAPI.Context;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning();
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("mydb"));
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddRateLimiter(x =>
{
    x.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 100;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});
builder.Services.AddCarter();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod());

app.UseRateLimiter();
app.MapCarter();
app.MapHealthChecks("/health");

app.Run();
