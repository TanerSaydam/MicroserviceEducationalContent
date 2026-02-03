using MassTransit;
using Microservice.Shared;

namespace Microservice.PaymentWebAPI;

public sealed class ProductDecreaseConsumer : IConsumer<ProductDecreaseMessage>
{
    public async Task Consume(ConsumeContext<ProductDecreaseMessage> context)
    {
        if (!context.Message.Result)
        {
            Console.WriteLine("Payment cancelled");
        }
    }
}
