namespace MyApp.Application.Features.Products;

/*
    API is implemented by infrastructure, no Reactive stuff here
*/

public sealed class ProductDetailDTO
{
    public string ProductNumber { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public string ProductDescription { get; init; } = null!;
    public decimal Price { get; init; }
    public int Version { get; init;  }
    public DateTimeOffset LastSavedAt { get; init; }
}

public interface IProductApi
{
    Task<int> SaveProductAsync(Product draft);

    Task PublishProductAsync(Guid productId);

    Task<ProductDetailDTO> LoadProductAsync(Guid productId);
}
