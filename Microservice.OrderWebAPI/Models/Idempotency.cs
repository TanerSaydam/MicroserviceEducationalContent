namespace Microservice.OrderWebAPI.Models;

public sealed class Idempotency
{
    public Guid Id { get; set; }
    public int Key { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}