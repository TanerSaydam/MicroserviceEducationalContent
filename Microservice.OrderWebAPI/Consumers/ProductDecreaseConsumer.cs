using MassTransit;
using Microservice.OrderWebAPI.Context;
using Microservice.OrderWebAPI.Enums;
using Microservice.Shared;
using Microsoft.EntityFrameworkCore;

namespace Microservice.OrderWebAPI.Consumers;

public sealed class ProductDecreaseConsumer
    (ApplicationDbContext dbContext) : IConsumer<ProductDecreaseMessage>
{
    public async Task Consume(ConsumeContext<ProductDecreaseMessage> context)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(p => p.Id == context.Message.OrderId);
        if (order != null)
        {
            order.Status = context.Message.Result ? OrderStatusEnum.Success : OrderStatusEnum.Fail;
            dbContext.Update(order);
            await dbContext.SaveChangesAsync();
        }
    }
}