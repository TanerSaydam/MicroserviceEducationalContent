using MassTransit;
using Microservice.OrderWebAPI.Consumers;
using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Dtos;
using Microservice.OrderWebAPI.Enums;
using Microservice.OrderWebAPI.Models;
using Microservice.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseInMemoryDatabase("MyDb");
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductDecreaseConsumer>();
    x.UsingRabbitMq((context, cfr) =>
    {
        cfr.Host("localhost", "/", h => { });
        cfr.ReceiveEndpoint("change-order-status", e =>
        {
            e.ConfigureConsumer<ProductDecreaseConsumer>(context);
        });
    });
});

var app = builder.Build();

#region Senkron
app.MapPost("/create-order", async (OrderCreateDto request, [FromHeader(Name = "idempotency-key")] int idempotencyKey, ApplicationDbContext dbContext, HttpClient httpClient) =>
{
    var isHaveIdempotency = await dbContext.Idempotencies.AnyAsync(i => i.Key == idempotencyKey);
    if (isHaveIdempotency)
    {
        return Results.Ok(new { Message = "Order was successful" });
    }

    #region Payment
    var message = await httpClient.GetAsync("http://payment:8080/pay");
    if (!message.IsSuccessStatusCode)
    {
        return Results.BadRequest("Something went wrong");
    }
    #endregion   

    #region Decrease Stock
    var productMessage = await httpClient.PutAsJsonAsync("http://payment:8080/pay", new { Id = request.ProductId, Quantity = 1 });
    if (!message.IsSuccessStatusCode)
    {
        return Results.BadRequest("Something went wrong");
    }
    #endregion

    #region Create Order
    var order = new Order()
    {
        ProductId = request.ProductId,
    };
    dbContext.Orders.Add(order);

    Idempotency idempotency = new()
    {
        Key = idempotencyKey,
        CreatedAt = DateTimeOffset.Now
    };
    dbContext.Idempotencies.Add(idempotency);
    await dbContext.SaveChangesAsync();
    #endregion

    return Results.Ok(new { Message = "Order was successful" });
});
#endregion

#region ASenkron
app.MapPost("/create-order-async", async (
    OrderCreateDto request,
    [FromHeader(Name = "idempotency-key")] int idempotencyKey,
    ApplicationDbContext dbContext,
    HttpClient httpClient,
    IPublishEndpoint publishEndpoint
    ) =>
{
    var isHaveIdempotency = await dbContext.Idempotencies.AnyAsync(i => i.Key == idempotencyKey);
    if (isHaveIdempotency)
    {
        return Results.Ok(new { Message = "Order was successful" });
    }

    #region Payment
    //var message = await httpClient.GetAsync("http://payment:8080/pay");
    //if (!message.IsSuccessStatusCode)
    //{
    //    return Results.BadRequest("Something went wrong");
    //}
    #endregion   

    #region Create Order
    var order = new Order()
    {
        ProductId = request.ProductId,
        Status = OrderStatusEnum.Pending
    };
    dbContext.Orders.Add(order);

    Idempotency idempotency = new()
    {
        Key = idempotencyKey,
        CreatedAt = DateTimeOffset.Now
    };
    dbContext.Idempotencies.Add(idempotency);
    await dbContext.SaveChangesAsync();
    #endregion

    var message = new CreateOrderMessage(order.Id, order.ProductId, request.Quantity);
    await publishEndpoint.Publish(message);

    return Results.Ok(new { Message = "Order create" });
});
#endregion

app.MapGet("/get-all", async (ApplicationDbContext dbContext, HttpClient httpClient) =>
{
    var orders = await dbContext.Orders.Select(s => new OrderDto
    {
        Id = s.Id,
        ProductId = s.ProductId,
        Status = s.Status.ToString()
    }).ToListAsync();

    var products = await httpClient.GetFromJsonAsync<List<ProductDto>>("http://localhost:5004/products/get-all");

    foreach (var order in orders)
    {
        //var productName = products!.First(p => p.Id == order.ProductId).Name;
        //order.ProductName = productName;
    }

    return Results.Ok(orders);
});

app.Run();