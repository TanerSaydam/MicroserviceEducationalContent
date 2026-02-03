using Microservice.OrderWebAPI.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Microservice.OrderWebAPI;

public class RabbitMqQueue
{
    public async Task SendAsync(OrderCreateDto request)
    {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // eğer bu exchange oluşmadıysa oluşturmak için
        await channel.ExchangeDeclareAsync(exchange: "create-order", type: ExchangeType.Fanout);

        var message = JsonSerializer.Serialize(request);
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: "create-order", routingKey: string.Empty, body: body);
    }
}

//durable: Queue’nun broker restart sonrası hayatta kalıp kalmayacağını belirler.
//false → RabbitMQ kapanırsa queue silinir
//true → Queue kalır (ama mesajların da kalması için publish ederken persistent gerekir)

//exclusive
//Queue’nun tek bir connection’a ait olup olmadığını belirler.
//true →
//Sadece bu connection kullanabilir
//Connection kapanınca queue otomatik silinir
//Genelde temporary / reply queue’lar için
//false → Normal, paylaşılabilir queue

//autoDelete
//Queue’nun son consumer disconnect olunca silinip silinmeyeceğini belirler.
//true → Consumer kalmayınca silinir
//false → Consumer olmasa bile queue durur