namespace Microservice.PaymentWebAPI.Dtos;

public sealed record PayDto(
    Guid OrderId,
    decimal Total);