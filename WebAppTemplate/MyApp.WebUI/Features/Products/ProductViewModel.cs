namespace MyApp.WebUI.Features.Products;

using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;
using MyApp.Application.Features.Products;
using MyApp.WebUI.Abstractions;

public sealed class ProductViewModel : ViewModelBase
{
    public AsyncState<Product> Product { get; }

    public ReactiveCommand<SaveProductCmd, ProductVersion> SaveProductRcmd { get; }

    public ReactiveCommand<PublishProductCmd, int> PublishRcmd { get; }

    public ReactiveCommand<LoadProductCmd, ProductDetailDTO> LoadRcmd { get; }

    public ProductViewModel(
        ILogger<ProductViewModel> logger,
        SaveProductEffect saveProductEffect,
        SaveProductReducer saveProductReducer,
        PublishProductEffect publishProductEffect,
        LoadProductEffect loadProductEffect)
    {
        Product = new AsyncState<Product>(new Product(string.Empty, string.Empty, string.Empty, 0, 1, DateTimeOffset.Now));
        Product.DisposeWith(this);

        SaveProductRcmd = ReactiveCommand.CreateWithReducer(
            state: Product,
            effect: saveProductEffect,
            reducer: saveProductReducer)
            .DisposeWith(this);

        LoadRcmd = new ReactiveCommand<LoadProductCmd, ProductDetailDTO>(
            effect: loadProductEffect,
            asyncState: Product,
            onResult: (cmd, result) =>
            {
                logger.LogInformation("Loaded product details, id: {Id}.", cmd.ProductId);
            })
            .DisposeWith(this);

        PublishRcmd = new ReactiveCommand<PublishProductCmd, int>(
            effect: publishProductEffect,
            asyncState: Product,
            onResult: (_, result) =>
            {
                logger.LogInformation("Published {Count} items.", result);
            })
            .DisposeWith(this);

        LoadRcmd
            .Results
            .Subscribe(dto =>
            {
                Product.Data.Update(_ => new Product(
                    ProductNumber: dto.ProductNumber,
                    ProductName: dto.ProductName,
                    Description: dto.ProductDescription,
                    Price: dto.Price,
                    Version: dto.Version,
                    LastSavedAt: DateTimeOffset.UtcNow));

                logger.LogInformation("Product data updated in ViewModel number: {ProductNumber}.", dto.ProductNumber);
            })
            .DisposeWith(this);
    }

    public void SaveProduct(Product draft) =>
        SaveProductRcmd.Execute(new SaveProductCmd(draft));

    public void PublishProduct(Guid productId) =>
        PublishRcmd.Execute(new PublishProductCmd(productId));

    public void LoadProduct(Guid productId) =>
        LoadRcmd.Execute(new LoadProductCmd(productId));
}
