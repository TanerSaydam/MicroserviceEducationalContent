using Microservice.ProductWebAPI.Context;
using Microservice.ProductWebAPI.Dtos;
using Microservice.ProductWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservice.ProductWebAPI.Services;

public sealed class ProductService(ApplicationDbContext dbContext)
{
    public async Task<bool> DecreaseStockAsync(ProductUpdateStockDto request, CancellationToken cancellationToken)
    {
        Product? product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        product.Stock -= request.Quantity;
        dbContext.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
