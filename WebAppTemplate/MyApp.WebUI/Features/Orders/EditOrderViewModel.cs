namespace MyApp.WebUI.Features.Orders;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;
using MyApp.WebUI.Abstractions;
using MyApp.Contracts.Orders;
using MyApp.Application.Features.Orders;

public sealed class EditOrderViewModel : ViewModelBase
{
    private readonly ReactiveState<OrderDraftDto> _state;
    private readonly Subject<Unit> _saveClicks = new();

    // private ReactiveCommand<LoadOrderCommand, Order?> LoadOrderRmd { get; }
    // private AsyncState<Order?> _order { get; }

    private ReactiveCommand<LoadOrderCategoriesCmd, IReadOnlyCollection<OrderCategory>> LoadCategoriesRmd { get; }
    private AsyncState<IReadOnlyCollection<OrderCategory>> _categories { get; }

    private readonly IDisposable _savePipeline;

    public EditOrderViewModel(
        ILogger<EditOrderViewModel> logger,
        LoadOrderEffect loadOrderEffect,
        LoadOrderCategoriesEffect loadCategoriesEffect,
        SaveOrderDraftEffect saveEffect)
    {
        // _order = new AsyncState<Order?>(null);
        // _order.DisposeWith(this);
        _categories = new AsyncState<IReadOnlyCollection<OrderCategory>>([]);
        _categories.DisposeWith(this);

        var address = new AddressDto("street", "city");
        var initial = new OrderDraftDto(address, [], [ Guid.Parse("22222222-2222-2222-2222-222222222222") ]);

        _state = new ReactiveState<OrderDraftDto>(initial);
        _state.DisposeWith(this);

        // LoadOrderRmd = new ReactiveCommand<LoadOrderCommand, Order?>(
        //     effect: loadOrderEffect,
        //     asyncState: _order,
        //     onResult: (cmd, result) =>
        //     {
        //         logger.LogInformation("Loaded order details, id: {Id}.", cmd.OrderId);
        //     })
        //     .DisposeWith(this);

        LoadCategoriesRmd = new ReactiveCommand<LoadOrderCategoriesCmd, IReadOnlyCollection<OrderCategory>>(
            effect: loadCategoriesEffect,
            asyncState: _categories)
            .DisposeWith(this);

        var isValidObservable = _state.Changes
            .Select(Validate)
            .DistinctUntilChanged();

        var canSaveObservable = _state.Changes
            .Select(s => !s.Equals(initial))
            .CombineLatest(isValidObservable, (c, v) => c && v)
            .DistinctUntilChanged();

        Subscribe(canSaveObservable,
            canSave =>
            {
                CanSave = canSave;
                RaiseStateChanged();
            });

        Subscribe(LoadCategoriesRmd.Results,
            categories =>
            {
                Categories = categories ?? [];
                RaiseStateChanged();
            });

        // Save effect pipeline
        _savePipeline =
            _saveClicks
                .WithLatestFrom(canSaveObservable, (_, canSave) => canSave)
                .Where(canSave => canSave)
                .SelectMany(_ =>
                    saveEffect.Handle(new SaveOrderDraftCommand(Guid.NewGuid(), _state.Value), CancellationToken.None)
                    .Retry(2)
                )
                .Subscribe();

        _saveClicks.DisposeWith(this);
        _savePipeline.DisposeWith(this);
    }

    public bool CanSave { get; private set; }
    public IReadOnlyCollection<OrderCategory> Categories { get; private set; } = [];
    public IList<Guid> SelectedCategoryIds
    {
        get => _state.Value.CategoryIds.ToList();
        set
        {
            _state.Update(s => s with { CategoryIds = value.ToList() });
            RaiseStateChanged();
        }
    }

    public void UpdateAddress(AddressDto address)
        => _state.Update(s => s with { Address = address });

    public void UpdateItems(IReadOnlyList<OrderItemDto> items)
        => _state.Update(s => s with { Items = items });

    public void Save()
        => _saveClicks.OnNext(Unit.Default);

    // public void LoadOrder(Guid orderId) =>
    //     LoadOrderRmd.Execute(new LoadOrderCommand(orderId));

    public void LoadCategories() =>
        LoadCategoriesRmd.Execute(new LoadOrderCategoriesCmd());

    private static bool Validate(OrderDraftDto dto)
        => dto.Items.Count > 0 && !string.IsNullOrWhiteSpace(dto.Address.Street);
}
