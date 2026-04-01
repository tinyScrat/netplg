namespace Lpc.Infrastructure.Services;

using System.Threading.Tasks;
using Lpc.Application.Features.Products;
using Microsoft.Extensions.Logging;
using Lpc.Application.Contracts;
using System.Net.Http.Json;

internal sealed class ProductApi(
    HttpClient http,
    ILogger<ProductApi> logger) : IProductApi
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

    public async Task<ProductDetailDTO> LoadProductDetailAsync(Guid productId, CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<ProductDetailDTO>("api/P001.json", ct)
            ?? throw new InvalidOperationException("Failed to load product details.");
    }

    public async Task<IEnumerable<ProductOverviewDTO>> LoadProductsAsync(CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<IEnumerable<ProductOverviewDTO>>("products.json", ct)
            ?? throw new InvalidOperationException("Failed to load products.");
    }
}
