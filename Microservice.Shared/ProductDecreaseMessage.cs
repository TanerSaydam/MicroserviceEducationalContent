namespace Microservice.Shared;

public sealed record ProductDecreaseMessage(
    Guid OrderId,
    bool Result);
