namespace Lpc.Presentation.Features;

using Microsoft.Extensions.Logging;
using Lpc.Application.Abstractions;
using Lpc.Application.Features.Products;
using Lpc.Presentation.Abstractions;
using Lpc.Application.Contracts;

public sealed record ProductRow
{
    public ProductOverviewDTO Product { get; init; } = null!;
    public bool IsValid { get; private set; } = true;
    public string? ValidationError { get; private set; }

    public void SetValidationResult(bool isValid, string? error)
    {
        IsValid = isValid;
        ValidationError = error;
    }

    public void ClearValidation()
    {
        IsValid = true;
        ValidationError = null;
    }
}

public sealed class ProductOverviewViewModel : ViewModelBase
{
    private readonly ILogger<ProductOverviewViewModel> logger;

    private readonly AsyncState<PagedResult<ProductRow>> products = new(PagedResult<ProductRow>.Empty);
    private readonly ReactiveCommand<LoadProductsCmd, PagedResult<ProductOverviewDTO>> loadProductsCmd;
    private readonly ReactiveCommand<CheckDuplicatedProductCmd, DuplicatedProductResultDTO> checkDuplicatedCmd;

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

        loadProductsCmd = new ReactiveCommand<LoadProductsCmd, PagedResult<ProductOverviewDTO>>(
            effect: loadProductsEffect,
            asyncState: products,
            onResult: (_, result) =>
            {
                var rows = result.Map(dto => new ProductRow { Product = dto });
                products.Data.Set(rows);
                logger.LogInformation("Loaded {Count} products.", result.TotalItems);
            })
            .DisposeWith(this);

        checkDuplicatedCmd = new ReactiveCommand<CheckDuplicatedProductCmd, DuplicatedProductResultDTO>(
            effect: new CheckDuplicatedProuductEffect(),
            asyncState: null,
            onResult: (_, result) =>
            {
                var productRow = products.Data.Value.Items.FirstOrDefault(r => r.Product.ProductId == result.ProductId);
                if (productRow != null && result.IsDuplicated)
                {
                    productRow.SetValidationResult(false, $"Duplicated with existing product: {result.ExistingProductNumber}");
                }
                else
                {
                    productRow?.ClearValidation();
                }

                //products.Data.Set(products.Data.Value); // Trigger UI update
                RaiseStateChanged();

                logger.LogInformation(
                    "Checked duplication for product {ProductId}: {IsDuplicated}",
                    result.ProductId,
                    result.IsDuplicated);
            })
            .DisposeWith(this);

        Subscribe(selectedProductIds.Changes, ids =>
        {
            // logger.LogInformation("Selected products: {Ids}", string.Join(", ", ids.Select(id => id.ToString("N"))));
        });
    }

    public IEnumerable<ProductRow> Products => products.Data.Value.Items;
    public bool IsLoading => products.Status.Value.IsLoading();
    public int TotalProducts => products.Data.Value.TotalItems;

    public void LoadProducts(int page = 1, int pageSize = 10)
    {
        logger.LogInformation("Loading products (Page: {Page}, PageSize: {PageSize})", page, pageSize);
        loadProductsCmd.Execute(new LoadProductsCmd(page, pageSize));
    }

    public bool IsSelected(ProductRow row)
        => selectedProductIds.Value.Contains(row.Product.ProductId);

    public bool? SelectionState
    {
        get
        {
            if (!Products.Any())
                return false;

            var selectedInPage =
                products.Data.Value.Items.Count(p => selectedProductIds.Value.Contains(p.Product.ProductId));

            if (selectedInPage == 0)
                return false;

            if (selectedInPage == products.Data.Value.Items.Count)
                return true;

            return null; // partially selected
        }
    }

    public void ToggleSelection(ProductRow row, bool selected)
    {
        var id = row.Product.ProductId;

        if (selected)
        {
            checkDuplicatedCmd.Execute(new CheckDuplicatedProductCmd(id));
        }
        else
        {
            // Clear validation when deselecting
            row.ClearValidation();
            //products.Data.Set(products.Data.Value); // Trigger UI update
            RaiseStateChanged();
        }

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

    public void SelectAll(bool selected)
    {
        logger.LogInformation("{Action} all products on current page.", selected ? "Selecting" : "Deselecting");
        
        if (!Products.Any()) return;

        selectedProductIds.Update(ids =>
        {
            if (selected)
                return [.. products.Data.Value.Items.Select(p => p.Product.ProductId)];
            else
                return [];
        });
    }
}
