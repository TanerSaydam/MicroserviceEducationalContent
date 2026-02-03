namespace Microservice.ProductWebAPI.Dtos;

public sealed record ProductCreateDto(
    string Name,
    Guid CategoryId);

public sealed record ProductUpdateDto(
    Guid Id,
    string Name,
    Guid CategoryId);

public sealed class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
}