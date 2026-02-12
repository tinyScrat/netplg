namespace MyApp.Application.Features.Products;

using MyApp.Application.Abstractions;
using System.Reactive.Linq;

/*
    Effect performs side effect with service provided by API
*/

public sealed record SaveProductCmd(Product Draft) : ICommand<ProductVersion>;

public sealed class SaveProductReducer
    : IReducer<Product, SaveProductCmd, ProductVersion>
{
    public Product Reduce(
        Product state,
        SaveProductCmd command,
        ProductVersion version)
    {
        return state with
        {
            Version = version.Version,
            LastSavedAt = DateTimeOffset.UtcNow
        };
    }
}

public sealed class SaveProductEffect(IProductApi api) : IEffect<SaveProductCmd, ProductVersion>
{
    public IObservable<ProductVersion> Handle(SaveProductCmd cmd, CancellationToken ct)
    {
        return Observable.FromAsync(async () =>
        {
            var version = await api.SaveProductAsync(cmd.Draft);

            return new ProductVersion(version);
        });
    }
}
