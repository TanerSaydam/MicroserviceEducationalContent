using Carter;
using Mapster;
using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Dtos;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Polly.Registry;
using Steeltoe.Common.Discovery;

namespace Microservice.ProductWebAPI.Modules;

public sealed class ProductModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        var app = group.MapGroup("products").WithTags("Products");

        app.MapGet(string.Empty, async (
            HttpClient httpClient,
            ApplicationDbContext dbContext,
            IDiscoveryClient discoveryClient,
            ResiliencePipelineProvider<string> pipelineProvider,
            CancellationToken cancellationToken) =>
        {
            var products = await dbContext.Products
            .OrderBy(p => p.Name)
            .Select(s => new ProductDto
            {
                Id = s.Id,
                Name = s.Name,
                CategoryId = s.CategoryId
            })
            .ToListAsync(cancellationToken);

            var services = await discoveryClient.GetInstancesAsync("CategoryWebAPI", cancellationToken);
            var service = services.FirstOrDefault();
            Uri uri = service?.Uri ?? new Uri("");
            string categoryEndpoint = uri + "categories";

            var pipeline = pipelineProvider.GetPipeline("http");
            var categories = await pipeline.ExecuteAsync(async st =>
            {
                return await httpClient.GetFromJsonAsync<List<CategoryDto>>(categoryEndpoint, cancellationToken);
            });

            foreach (var product in products)
            {
                product.CategoryName = categories?.FirstOrDefault(i => i.Id == product.CategoryId)?.Name ?? "";
            }

            return products;
        });

        app.MapPost(string.Empty, async (
            ProductCreateDto request,
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            Product product = request.Adapt<Product>();
            dbContext.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new { Message = "Product create was succesful" };
        });

        app.MapPost("seed-data", async (
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            List<Product> products = new();
            for (int i = 0; i < 1000; i++)
            {
                Product product = new()
                {
                    Name = "Product " + i.ToString(),
                    CategoryId = new Guid("019c2075-97e2-7bc4-80f5-aaa77478b070")
                };
                products.Add(product);
            }

            dbContext.AddRange(products);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new { Message = "Product seed data was succesful" };
        });
    }
}