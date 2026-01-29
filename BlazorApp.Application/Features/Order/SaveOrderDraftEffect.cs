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
            // var res = await _http.PostAsJsonAsync(
            //     $"/orders/{cmd.OrderId}/update-draft",
            //     dto
            // );

            // res.EnsureSuccessStatusCode();

            await api.SaveOrderAsync(cmd.Draft);

            return Unit.Default;
        });
    }
}
