using Carter;
using Mapster;
using Microservice.CategoryWebAPI.Context;
using Microservice.CategoryWebAPI.Dtos;
using Microservice.CategoryWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Microservice.CategoryWebAPI.Modules;

public sealed class CategoryModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        //var versionSet = group.NewApiVersionSet()
        //    //.HasApiVersion(new ApiVersion(1))
        //    .HasDeprecatedApiVersion(new ApiVersion(1))
        //    .HasApiVersion(new ApiVersion(2))
        //    .ReportApiVersions()
        //    .Build();

        var app = group
            //.MapGroup("/v{version:apiVersion}/categories")
            .MapGroup("categories")
            .WithTags("Categories")
            .RequireRateLimiting("fixed");
        //.WithApiVersionSet(versionSet);

        app.MapPost(string.Empty, async (CategoryCreateDto request, ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var isNameExists = await dbContext.Categories.AnyAsync(i => i.Name == request.Name, cancellationToken);
            if (isNameExists)
            {
                return Results.BadRequest(new { Message = "Category name already exists" });
            }

            Category category = request.Adapt<Category>();
            category.Id = new Guid("019c2075-97e2-7bc4-80f5-aaa77478b070");
            dbContext.Add(category);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { Message = "(V1) Category create was successful" });
        });//.HasApiVersion(1);

        //app.MapPost(string.Empty, async (CategoryCreateDto request, ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
        //{
        //    //do something else...

        //    return Results.Ok(new { Message = "(V2) Category create was successful" });
        //}).HasApiVersion(2);

        app.MapGet(string.Empty, async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var res = await dbContext.Categories.OrderBy(i => i.Name).ToListAsync(cancellationToken);

            return Results.Ok(res);
        });//.HasApiVersion(1);

        //app.MapGet(string.Empty, async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
        //{
        //    var res = await dbContext.Categories.OrderBy(i => i.Name).ToListAsync(cancellationToken);

        //    return Results.Ok(res);
        //}).HasApiVersion(2);
    }
}