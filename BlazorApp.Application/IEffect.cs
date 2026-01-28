namespace BlazorApp.Application;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

public interface IEffect<TCommand>
{
    IObservable<Unit> Handle(TCommand command);
}

public sealed class ObservableState<T>(T initial) : IDisposable
{
    private readonly BehaviorSubject<T> _subject = new BehaviorSubject<T>(initial);

    public T Value => _subject.Value;

    public IObservable<T> Changes => _subject.AsObservable();

    public void Update(Func<T, T> updater)
    {
        _subject.OnNext(updater(_subject.Value));
    }

    public void Dispose() => _subject.Dispose();
}

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

public sealed record SaveOrderDraftCommand(Guid OrderId);

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
                    saveEffect.Handle(new SaveOrderDraftCommand(Guid.NewGuid()))
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

    public OrderDraftDto ToDraftDto() => _state.Value;

    private static bool Validate(OrderDraftDto dto)
        => dto.Items.Count > 0 && !string.IsNullOrWhiteSpace(dto.Address.Street);

    public void Dispose()
    {
        _savePipeline.Dispose();
        _state.Dispose();
        _saveClicks.Dispose();
    }
}


public interface IOrderApi
{
    Task SaveOrderAsync(OrderDraftDto dto);
}

public sealed class SaveOrderDraftEffect(IOrderApi api, EditOrderViewModel vm) : IEffect<SaveOrderDraftCommand>
{
    public IObservable<Unit> Handle(SaveOrderDraftCommand cmd)
    {
        return Observable.FromAsync(async () =>
        {
            var dto = vm.ToDraftDto();

            // var res = await _http.PostAsJsonAsync(
            //     $"/orders/{cmd.OrderId}/update-draft",
            //     dto
            // );

            // res.EnsureSuccessStatusCode();

            await api.SaveOrderAsync(dto);

            return Unit.Default;
        });
    }
}
