namespace Lpc.Application.Features.Orders;

using System.Reactive.Linq;
using Lpc.Application.Abstractions;
using Lpc.Contracts;
using Lpc.Contracts.Orders;

public sealed record LoadOrderCommand(Guid OrderId) : ICommand<Order?>;

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

public sealed record LoadOrdersCmd() : ICommand<PagedResponse<OrderOverview>>;

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


public sealed record LoadOrderCategoriesCmd() : ICommand<IReadOnlyCollection<OrderCategory>>;

public sealed class LoadOrderCategoriesEffect(IOrderApi api) : IEffect<LoadOrderCategoriesCmd, IReadOnlyCollection<OrderCategory>>
{
    public IObservable<IReadOnlyCollection<OrderCategory>> Handle(LoadOrderCategoriesCmd command, CancellationToken ct = default)
    {
        return Observable.FromAsync(async () =>
        {
            var dtos = await api.GetOrderCategoriesAsync(ct);
            return dtos.Select(dto => new OrderCategory(dto.Id, dto.Name)).ToArray();
        });
    }
}
