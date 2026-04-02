namespace Lpc.Application.Features.Products;

using Lpc.Application.Contracts;

public interface IProductApi
{
    Task<int> SaveProductAsync(Product draft);

    Task PublishProductAsync(Guid productId);

    Task<ProductDetailDTO> LoadProductDetailAsync(Guid productId, CancellationToken ct = default);
    Task<PagedResult<ProductOverviewDTO>> LoadProductsAsync(int page, int pageSize, CancellationToken ct = default);
}
