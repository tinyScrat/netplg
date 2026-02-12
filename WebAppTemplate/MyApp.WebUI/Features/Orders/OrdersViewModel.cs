namespace MyApp.WebUI.Features.Orders;

using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;
using MyApp.WebUI.Abstractions;
using MyApp.Contracts;
using MyApp.Contracts.Orders;
using MyApp.Application.Features.Orders;

public sealed class OrdersViewModel : ViewModelBase
{
    public AsyncState<PagedResponse<OrderOverview>> Orders { get; }

    public ReactiveCommand<LoadOrdersCmd, PagedResponse<OrderOverview>> LoadOrdersRmd { get; }

    public OrdersViewModel(
        LoadOrdersEffect loadOrdersEffect,
        ILogger<OrdersViewModel> logger)
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

    public void LoadOrders()
    {
        LoadOrdersRmd.Execute(new LoadOrdersCmd());
    }
}
