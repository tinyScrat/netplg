namespace Lpc.Application.Features.Products;

using Lpc.Application.Contracts;

public interface IProductApi
{
    Task<int> SaveProductAsync(Product draft);

    Task PublishProductAsync(Guid productId);

    Task<ProductDetailDTO> LoadProductAsync(Guid productId);
}
