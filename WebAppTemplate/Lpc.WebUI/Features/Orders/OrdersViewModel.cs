namespace Lpc.WebUI.Features.Orders;

using Microsoft.Extensions.Logging;
using Lpc.Application.Abstractions;
using Lpc.WebUI.Abstractions;
using Lpc.Contracts;
using Lpc.Contracts.Orders;
using Lpc.Application.Features.Orders;

public sealed class OrdersViewModel : ViewModelBase
{
    public AsyncState<PagedResponse<OrderOverview>> Orders { get; }

    public ReactiveCommand<LoadOrdersCmd, PagedResponse<OrderOverview>> LoadOrdersRmd { get; }

    public OrdersViewModel(
        LoadOrdersEffect loadOrdersEffect,
        ILogger<OrdersViewModel> logger,
        GlobalErrorStore errorStore) : base(errorStore)
    {
        Orders = new AsyncState<PagedResponse<OrderOverview>>(
            new PagedResponse<OrderOverview>
            {
                Page = 1,
                PageSize = 100,
                TotalItems = 0,
                TotalPages = 0,
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

        LoadOrdersRmd = new ReactiveCommand<LoadOrdersCmd, PagedResponse<OrderOverview>>(
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

    public PagedResponse<OrderOverview> OrdersData { get; private set; } = PagedResponse<OrderOverview>.Empty;

    public void LoadOrders()
    {
        LoadOrdersRmd.Execute(new LoadOrdersCmd());
    }
}
