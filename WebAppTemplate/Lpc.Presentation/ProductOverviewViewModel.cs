namespace Lpc.Presentation.Features;

using Microsoft.Extensions.Logging;
using Lpc.Application.Abstractions;
using Lpc.Application.Features.Products;
using Lpc.Presentation.Abstractions;
using Lpc.Application.Contracts;

public sealed record ProductRow(ProductOverviewDTO Product, bool IsSelected);

public sealed class ProductOverviewViewModel : ViewModelBase
{
    private readonly ILogger<ProductOverviewViewModel> logger;

    private readonly AsyncState<IEnumerable<ProductRow>> products = new([]);
    private readonly ReactiveCommand<LoadProductsCmd, IEnumerable<ProductOverviewDTO>> loadProductsCmd;

    private readonly ReactiveState<HashSet<Guid>> selectedProductIds = new([]);

    public ProductOverviewViewModel(
        LoadProductsEffect loadProductsEffect,
        ILogger<ProductOverviewViewModel> logger,
        GlobalErrorStore errorStore) : base(errorStore)
    {
        this.logger = logger;

        products.DisposeWith(this);
        selectedProductIds.DisposeWith(this);

        ObserveState(
            products.Data.Signal,
            products.Status.Signal,
            selectedProductIds.Signal);

        loadProductsCmd = new ReactiveCommand<LoadProductsCmd, IEnumerable<ProductOverviewDTO>>(
            effect: loadProductsEffect,
            asyncState: products,
            onResult: (_, result) =>
            {
                products.Data.Set(result.Select(p => new ProductRow(p, selectedProductIds.Value.Contains(p.ProductId))));
                logger.LogInformation("Loaded {Count} products.", result.Count());
            })
            .DisposeWith(this);

        Subscribe(selectedProductIds.Changes, ids =>
        {
            logger.LogInformation("Selected products: {Ids}", string.Join(", ", ids.Select(id => id.ToString("N"))));
        });
    }

    public IEnumerable<ProductRow> Products => products.Data.Value;
    public bool IsLoading => products.Status.Value.IsLoading();
    public int TotalProducts => products.Data.Value.Count();

    public void LoadProducts()
    {
        loadProductsCmd.Execute(new LoadProductsCmd());
    }

    public bool IsSelected(ProductRow row)
        => selectedProductIds.Value.Contains(row.Product.ProductId);

    public void ToggleSelection(ProductRow row, bool selected)
    {
        logger.LogInformation("{Action} product {Id}.", selected ? "Selected" : "Deselected", row.Product.ProductId.ToString("N"));
        
        var id = row.Product.ProductId;

        selectedProductIds.Update(ids =>
        {
            var newSet = new HashSet<Guid>(ids);

            if (selected)
                newSet.Add(id);
            else
                newSet.Remove(id);

            return newSet;
        });
    }
}
