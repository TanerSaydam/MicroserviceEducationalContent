namespace Microservice.ProductWebAPI.Dtos;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    int Stock);