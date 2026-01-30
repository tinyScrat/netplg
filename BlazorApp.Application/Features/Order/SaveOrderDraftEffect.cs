namespace BlazorApp.Application.Features.Orders;

using BlazorApp.Application.Abstractions;
using System.Reactive;
using System.Reactive.Linq;

public sealed class SaveOrderDraftEffect(IOrderApi api) : IEffect<SaveOrderDraftCommand>
{
    public IObservable<Unit> Handle(SaveOrderDraftCommand cmd)
    {
        return Observable.FromAsync(async () =>
        {
            await api.SaveOrderAsync(cmd.Draft);

            return Unit.Default;
        });
    }
}
