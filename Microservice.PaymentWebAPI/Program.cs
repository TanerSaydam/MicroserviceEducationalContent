using MassTransit;
using Microservice.PaymentWebAPI;
using Microservice.PaymentWebAPI.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductDecreaseConsumer>();
    x.UsingRabbitMq((context, cfr) =>
    {
        cfr.Host("localhost", "/", h => { });
        cfr.ReceiveEndpoint("change-payment-status", e =>
        {
            e.ConfigureConsumer<ProductDecreaseConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapPost("/pay", (PayDto request) =>
{
    return Results.Ok(new { Message = "Payment was successful" });
});

app.Run();