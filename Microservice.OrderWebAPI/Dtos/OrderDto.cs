namespace Microservice.OrderWebAPI.Dtos;

public sealed class OrderDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public string Status { get; set; } = default!;
}
