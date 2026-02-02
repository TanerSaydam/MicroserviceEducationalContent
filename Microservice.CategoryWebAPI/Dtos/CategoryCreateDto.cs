namespace Microservice.CategoryWebAPI.Dtos;

public sealed record CategoryCreateDto(
    string Name);

public sealed record CategoryUpdateDto(
    Guid Id,
    string Name);