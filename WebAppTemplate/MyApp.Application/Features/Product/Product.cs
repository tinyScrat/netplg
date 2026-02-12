namespace MyApp.Application.Features.Products;

public sealed record Product(
    string ProductNumber,
    string ProductName,
    string Description,
    decimal Price,
    int Version,
    DateTimeOffset LastSavedAt);

public sealed record ProductVersion(int Version);
