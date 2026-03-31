namespace Lpc.Application.Features.Orders;

using Lpc.Application.Abstractions;
using Lpc.Contracts.Orders;
using System.Reactive;
using System.Reactive.Linq;

public sealed record SaveOrderDraftCommand(Guid OrderId, OrderDraftDto Draft) : ICommand<Unit>;

public sealed class SaveOrderDraftEffect(IOrderApi api) : IEffect<SaveOrderDraftCommand, Unit>
{
    public IObservable<Unit> Handle(SaveOrderDraftCommand cmd, CancellationToken ct)
    {
        return Observable.FromAsync(async () =>
        {
            await api.SaveOrderAsync(cmd.Draft);

            return Unit.Default;
        });
    }
}
