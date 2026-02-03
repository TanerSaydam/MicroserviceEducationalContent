using Carter;
using Microservice.ProductWebAPI.Context;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Scalar.AspNetCore;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("MyDb");
});
builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddHttpClient();
builder.Services.AddConsulDiscoveryClient();
builder.Services.AddResponseCompression(x => x.EnableForHttps = true);
builder.Services.AddResiliencePipeline("http", configure =>
{
    configure.AddPipeline(new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions()
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(3),
            ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>()
        })
        .Build()
        );
});
var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod());

app.MapCarter();

app.MapGet(string.Empty, () => "Hello world");

app.UseResponseCompression();

app.Run();