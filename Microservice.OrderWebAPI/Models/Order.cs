using Microservice.OrderWebAPI.Enums;

namespace Microservice.OrderWebAPI.Models;

public sealed class Order
{
    public Order()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public OrderStatusEnum Status { get; set; }
}