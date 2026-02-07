namespace MyApp.Infrastructure.Features.Products;

using System.Threading.Tasks;
using MyApp.Application.Features.Products;
using Microsoft.Extensions.Logging;

internal sealed class ProductApi(ILogger<ProductApi> logger) : IProductApi
{
    public Task<int> SaveProductAsync(Product draft)
    {
        logger.LogInformation("Product {Name} Saved", draft.ProductName);
        return Task.FromResult(1);
    }

    public Task PublishProductAsync(Guid productId)
    {
        logger.LogInformation("Product {Id} Published", productId.ToString("N"));
        return Task.CompletedTask;
    }

    public async Task<ProductDetailDTO> LoadProductAsync(Guid productId)
    {
        await Task.Delay(1000);

        return new ProductDetailDTO
        {
            ProductNumber = "P0001",
            ProductName = "iPhone 17",
            ProductDescription = "Design by Apple, made in China",
            Price = 5800,
            Version = 3,
            LastSavedAt = DateTimeOffset.UtcNow.AddDays(1)
        };
    }
}
