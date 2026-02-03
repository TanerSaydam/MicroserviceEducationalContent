namespace Microservice.Shared;

public sealed record CreateOrderMessage(
    Guid OrderId,
    Guid ProductId,
    int Quantity);
