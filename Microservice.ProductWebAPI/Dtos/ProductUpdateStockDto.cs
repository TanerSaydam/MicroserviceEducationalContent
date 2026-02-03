namespace Microservice.ProductWebAPI.Dtos;

public sealed record ProductUpdateStockDto(
    Guid Id,
    int Quantity);
