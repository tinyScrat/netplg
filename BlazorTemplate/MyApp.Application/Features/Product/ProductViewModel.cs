namespace MyApp.Application.Features.Products;

using MyApp.Application.Abstractions;
using System.Reactive;
using System.Reactive.Linq;

public sealed record Product(
    string ProductNumber,
    string ProductName,
    string Description,
    decimal Price,
    int Version,
    DateTimeOffset LastSavedAt);

public sealed record ProductVersion(int Version);
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


public sealed record PublishProductCmd(Guid ProductId) : ICommand<int>;

public sealed record LoadProductCmd(Guid ProductId) : ICommand<ProductDetailDTO>;

public sealed class ProductViewModel : ViewModelBase
{
    public AsyncState<Product> Product { get; }

    public ReactiveCommand<SaveProductCmd, ProductVersion> SaveProduct { get; }

    // public AsyncCommand<PublishProductCmd, int> Publish { get; }
    // public AsyncCommand<LoadProductCmd, ProductDetailDTO> Load { get; }

    public ProductViewModel(
        SaveProductEffect saveProductEffect,
        SaveProductReducer saveProductReducer,
        PublishProductEffect publishProductEffect)
    {
        Product = new AsyncState<Product>(new Product(string.Empty, string.Empty, string.Empty, 0, 1, DateTimeOffset.Now));

        var dispatcher = new CommandDispatcher<Product>(Product);

        SaveProduct = ReactiveCommand.CreateFromObservable<Product, SaveProductCmd, ProductVersion>(
            commandFactory: draft => new SaveProductCmd(draft),
            execute: command =>
                dispatcher.Dispatch(
                    command,
                    saveProductEffect,
                    saveProductReducer
                )//,
            //asyncState: saveProductState
        );


        // var saveState = new AsyncState<ProductVersion>(new ProductVersion(Product.Data.Value.Version));
        // Save = new AsyncCommand<SaveProductCmd, ProductVersion>(saveProductEffect, saveState, version =>
        // {
        //     Console.WriteLine($"Saved. New version: {version}");
        // });

        // Save.DisposeWith(this);

        // Publish = new AsyncCommand<PublishProductCmd, int>(publishProductEffect.Handle);
        // Publish.DisposeWith(this);

        // Load = new AsyncCommand<LoadProductCmd, ProductDetailDTO>()

        // Save.Changes
        //     .OfType<AsyncState<Product>.Success>()
        //     .Select(s => s.Value)
        //     .Subscribe(Product.Set)
        //     .DisposeWith(this);

        // Publish.Changes
        //     .OfType<AsyncState<Product>.Success>()
        //     .Select(s => s.Value)
        //     .Subscribe(Product.Set)
        //     .DisposeWith(this);
    }

    // public void SaveProduct(Guid id)
    // {
    //     Save.Execute(new SaveProductCmd(id));
    // }
}
