using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Dtos;
using Microservice.OrderWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseInMemoryDatabase("MyDb");
});

var app = builder.Build();

app.MapPost("/create-order", async (OrderCreateDto request, [FromHeader(Name = "idempotency-key")] int idempotencyKey, ApplicationDbContext dbContext, HttpClient httpClient) =>
{
    var isHaveIdempotency = await dbContext.Idempotencies.AnyAsync(i => i.Key == idempotencyKey);
    if (isHaveIdempotency)
    {
        return Results.Ok(new { Message = "Order was successful" });
    }

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

    var message = await httpClient.GetAsync("http://payment:8080/pay");
    if (!message.IsSuccessStatusCode)
    {
        return Results.BadRequest("Something went wrong");
    }

    return Results.Ok(new { Message = "Order was successful" });
});

app.MapGet("/get-all", async (ApplicationDbContext dbContext, HttpClient httpClient) =>
{
    var orders = await dbContext.Orders.Select(s => new OrderDto
    {
        Id = s.Id,
        ProductId = s.ProductId,
    }).ToListAsync();

    var products = await httpClient.GetFromJsonAsync<List<ProductDto>>("http://product1:8080/products/get-all");

    foreach (var order in orders)
    {
        var productName = products!.First(p => p.Id == order.ProductId).Name;
        order.ProductName = productName;
    }

    return Results.Ok(orders);
});

app.Run();