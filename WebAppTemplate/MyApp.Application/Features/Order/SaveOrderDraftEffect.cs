namespace MyApp.Application.Features.Orders;

using MyApp.Application.Abstractions;
using System.Reactive;
using System.Reactive.Linq;

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
