using Carter;
using MassTransit;
using Microservice.ProductWebAPI.Consumers;
using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("MyDb");
});
builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddCarter();
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();
//builder.Services.AddConsulDiscoveryClient();
builder.Services.AddTransient<ProductService>();
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

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderConsumer>();

    x.UsingRabbitMq((context, cfr) =>
    {
        cfr.Host("localhost", "/", h => { });
        cfr.ReceiveEndpoint("product-decrease", e =>
        {
            e.ConfigureConsumer<OrderConsumer>(context);
        });
    });
});

var app = builder.Build();


app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors(x => x
.AllowAnyHeader()
.AllowAnyOrigin()
.AllowAnyMethod());

app.MapCarter();

app.MapHealthChecks("health");

app.UseResponseCompression();

app.Run();