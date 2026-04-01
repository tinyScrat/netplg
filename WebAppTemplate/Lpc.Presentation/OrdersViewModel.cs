namespace Lpc.Presentation.Features;

using Microsoft.Extensions.Logging;
using Lpc.Application.Abstractions;
using Lpc.Application.Contracts;
using Lpc.Contracts.Orders;
using Lpc.Application.Features.Orders;
using Lpc.Presentation.Abstractions;

public sealed class OrdersViewModel : ViewModelBase
{
    public AsyncState<PagedResult<OrderOverview>> Orders { get; }

    public ReactiveCommand<LoadOrdersCmd, PagedResult<OrderOverview>> LoadOrdersRmd { get; }

    public OrdersViewModel(
        LoadOrdersEffect loadOrdersEffect,
        ILogger<OrdersViewModel> logger,
        GlobalErrorStore errorStore) : base(errorStore)
    {
        Orders = new AsyncState<PagedResult<OrderOverview>>(
            new PagedResult<OrderOverview>
            {
                Page = 1,
                PageSize = 100,
                TotalItems = 0,
                Items = []
            }
        );
        Orders.DisposeWith(this);

        Subscribe(Orders.Status.Changes, status =>
        {
            IsLoading = status.IsLoading();
            RaiseStateChanged();
        });

        Subscribe(Orders.Data.Changes, data =>
        {
            OrdersData = data;
            RaiseStateChanged();
        });

        LoadOrdersRmd = new ReactiveCommand<LoadOrdersCmd, PagedResult<OrderOverview>>(
            loadOrdersEffect,
            Orders,
            (cmd, response) =>
            {
                logger.LogInformation("Loaded {Count} orders", response.Items.Count);
            }
        );
        LoadOrdersRmd.DisposeWith(this);

        LoadOrdersRmd.Results.Subscribe(response =>
        {
            Orders.Data.Update(res => response);
        })
        .DisposeWith(this);
    }

    public bool IsLoading { get; private set; } = false;

    public PagedResult<OrderOverview> OrdersData { get; private set; } = PagedResult<OrderOverview>.Empty;

    public void LoadOrders()
    {
        LoadOrdersRmd.Execute(new LoadOrdersCmd());
    }
}
