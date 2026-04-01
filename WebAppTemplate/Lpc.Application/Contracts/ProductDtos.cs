namespace Lpc.Application.Contracts;

public sealed class ProductDetailDTO
{
    public string ProductNumber { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public string ProductDescription { get; init; } = null!;
    public decimal Price { get; init; }
    public int Version { get; init;  }
    public DateTimeOffset LastSavedAt { get; init; }
}

public sealed class ProductOverviewDTO
{
    public Guid ProductId { get; init; }
    public string ProductNumber { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public decimal Price { get; init; }
    public string Currency { get; init; } = null!;
}
