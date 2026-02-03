using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Microservice.ProductWebAPI;

public sealed class OrderQueueBackgroundService(IServiceScopeFactory serviceScope) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // eğer bu exchange oluşmadıysa oluşturmak için
        await channel.ExchangeDeclareAsync(exchange: "create-order",
            type: ExchangeType.Fanout);

        // exchange'e bağlanıyoruz
        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync(durable: true);
        string queueName = queueDeclareResult.QueueName;
        await channel.QueueBindAsync(queue: queueName, exchange: "create-order", routingKey: string.Empty);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            CreateOrderDto? orderDto = JsonSerializer.Deserialize<CreateOrderDto>(message);
            if (orderDto is not null)
            {
                using var scoped = serviceScope.CreateScope();
                var dbContext = scoped.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Product? product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == orderDto.ProductId);
                if (product is not null)
                {
                    product.Stock -= 1;
                    dbContext.Update(product);
                    await dbContext.SaveChangesAsync();

                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
        };

        await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer);

    }
}


public sealed record CreateOrderDto(
    Guid ProductId);