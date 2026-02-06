namespace MyApp.Application.Features.Orders;

using System.Reactive.Linq;
using MyApp.Application.Abstractions;
using MyApp.Contracts;
using MyApp.Contracts.Orders;

public sealed class LoadOrderEffect(IOrderApi api) 
    : IEffect<LoadOrderCommand, Order?>
{
    public IObservable<Order?> Handle(LoadOrderCommand command, CancellationToken ct)
    {
        return Observable.FromAsync(() => api.TryGetOrderAsync(command.OrderId, ct))
            .SelectMany(orderDto =>
            {
                if (orderDto is null)
                    return Observable.Return<Order?>(null);

                return Observable.FromAsync(() => api.GetOrderLinesAsync(command.OrderId, ct))
                    .Select(lineDtos => OrderMappings.Compose(orderDto, lineDtos));
            })
            .RetryWithBackoff(
                maxRetries: 3,
                initialDelay: TimeSpan.FromMilliseconds(500),
                factor: 2.0);
    }
}

public sealed class LoadOrdersEffect(IOrderApi api) : IEffect<LoadOrdersCmd, PagedResponse<OrderOverview>>
{
    public IObservable<PagedResponse<OrderOverview>> Handle(LoadOrdersCmd command, CancellationToken ct = default)
    {
        return Observable.FromAsync(async () =>
        {
            return await api.GetOrdersAsync(ct);
        });
    }
}
