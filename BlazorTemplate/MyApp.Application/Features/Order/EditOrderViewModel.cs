namespace MyApp.Application.Features.Orders;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MyApp.Application.Abstractions;

public sealed record AddressDto(
    string Street,
    string City
);

public sealed record OrderItemDto(
    string ProductId,
    int Quantity
);

public sealed record OrderDraftDto(
    AddressDto Address,
    IReadOnlyList<OrderItemDto> Items
);

public sealed record SaveOrderDraftCommand(Guid OrderId, OrderDraftDto Draft) : ICommand;

public sealed class EditOrderViewModel : IDisposable
{
    private readonly ObservableState<OrderDraftDto> _state;
    private readonly Subject<Unit> _saveClicks = new();

    private readonly IDisposable _savePipeline;

    public EditOrderViewModel(
        OrderDraftDto initial,
        SaveOrderDraftEffect saveEffect)
    {
        _state = new ObservableState<OrderDraftDto>(initial);

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
                    saveEffect.Handle(new SaveOrderDraftCommand(Guid.NewGuid(), _state.Value))
                    .Retry(2)
                )
                .Subscribe();
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

    // ---- Mapping ----
    private static bool Validate(OrderDraftDto dto)
        => dto.Items.Count > 0 && !string.IsNullOrWhiteSpace(dto.Address.Street);

    public void Dispose()
    {
        _savePipeline.Dispose();
        _state.Dispose();
        _saveClicks.Dispose();
    }
}
