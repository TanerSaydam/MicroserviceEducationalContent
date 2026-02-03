using MassTransit;
using Microservice.ProductWebAPI.Dtos;
using Microservice.ProductWebAPI.Services;
using Microservice.Shared;

namespace Microservice.ProductWebAPI.Consumers;

public sealed class OrderConsumer(
    ProductService productService,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<CreateOrderMessage>
{
    public async Task Consume(ConsumeContext<CreateOrderMessage> context)
    {
        var request = new ProductUpdateStockDto(context.Message.ProductId, context.Message.Quantity);
        var res = await productService.DecreaseStockAsync(request, default);

        var result = new ProductDecreaseMessage(context.Message.OrderId, res);
        await publishEndpoint.Publish(result);
    }
}
