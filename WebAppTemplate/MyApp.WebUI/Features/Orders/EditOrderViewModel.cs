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

    public ReactiveCommand<LoadOrderCommand, Order?> LoadOrderRmd { get; }
    public AsyncState<Order?> Order { get; }

    private readonly IDisposable _savePipeline;

    public EditOrderViewModel(
        ILogger<EditOrderViewModel> logger,
        LoadOrderEffect loadOrderEffect,
        SaveOrderDraftEffect saveEffect)
    {
        Order = new AsyncState<Order?>(null);
        Order.DisposeWith(this);

        var address = new AddressDto("street", "city");
        var initial = new OrderDraftDto(address, []);

        _state = new ReactiveState<OrderDraftDto>(initial);
        _state.DisposeWith(this);

        LoadOrderRmd = new ReactiveCommand<LoadOrderCommand, Order?>(
            effect: loadOrderEffect,
            asyncState: Order,
            onResult: (cmd, result) =>
            {
                logger.LogInformation("Loaded order details, id: {Id}.", cmd.OrderId);
            })
            .DisposeWith(this);

        // Derived state
        HasChanges = _state.Changes
            .Select(s => !s.Equals(initial))
            .DistinctUntilChanged();

        IsValid = _state.Changes
            .Select(Validate)
            .DistinctUntilChanged();

        CanSave = HasChanges
            .CombineLatest(IsValid, (c, v) => c && v)
            .DistinctUntilChanged();

        // Save effect pipeline
        _savePipeline =
            _saveClicks
                .WithLatestFrom(CanSave, (_, canSave) => canSave)
                .Where(canSave => canSave)
                .SelectMany(_ =>
                    saveEffect.Handle(new SaveOrderDraftCommand(Guid.NewGuid(), _state.Value), CancellationToken.None)
                    .Retry(2)
                )
                .Subscribe();

        _saveClicks.DisposeWith(this);
        _savePipeline.DisposeWith(this);
    }

    // ---- Reactive outputs ----

    public IObservable<bool> HasChanges { get; }
    public IObservable<bool> IsValid { get; }
    public IObservable<bool> CanSave { get; }

    // ---- UI entry points ----

    public void UpdateAddress(AddressDto address)
        => _state.Update(s => s with { Address = address });

    public void UpdateItems(IReadOnlyList<OrderItemDto> items)
        => _state.Update(s => s with { Items = items });

    public void Save()
        => _saveClicks.OnNext(Unit.Default);

    public void LoadOrder(Guid orderId) =>
        LoadOrderRmd.Execute(new LoadOrderCommand(orderId));

    // ---- Mapping ----
    private static bool Validate(OrderDraftDto dto)
        => dto.Items.Count > 0 && !string.IsNullOrWhiteSpace(dto.Address.Street);
}
