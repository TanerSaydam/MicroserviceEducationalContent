using Asp.Versioning;
using Carter;
using Microservice.CategoryWebAPI.Context;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Steeltoe.Discovery.Consul;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(action =>
{
    action.DefaultApiVersion = new ApiVersion(1);
    action.AssumeDefaultVersionWhenUnspecified = true;
    action.ReportApiVersions = true;
})
    .AddApiExplorer(options =>
    {
        //openapi için lazým
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("mydb"));
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddControllers();
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
builder.Services.AddConsulDiscoveryClient();

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
app.MapControllers();

app.Run();
