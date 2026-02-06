namespace MyApp.Infrastructure.Features.Orders;

using MyApp.Contracts.Orders;
using System.Threading.Tasks;
using MyApp.Application.Features.Orders;
using Microsoft.Extensions.Logging;
using MyApp.Contracts;

// using System.Net;
// using System.Net.Http.Json;

internal sealed class OrderApi(
    /*HttpClient http,*/
    ILogger<OrderApi> logger) : IOrderApi
{
    public Task SaveOrderAsync(OrderDraftDto dto)
    {
        logger.LogInformation("Order saved");
        return Task.CompletedTask;
    }

    public async Task<OrderDto?> TryGetOrderAsync(Guid orderId, CancellationToken ct)
    {
        // var response = await http.GetAsync($"/orders/{orderId}");
        // if (response.StatusCode == HttpStatusCode.NotFound)
        //     return null;

        // response.EnsureSuccessStatusCode();
        // return await response.Content.ReadFromJsonAsync<OrderDto>();

        await Task.Delay(100);

        return new OrderDto
        {
            Id = orderId,
            OrderNumber = "ORD-2026-0001",
            CreatedAt = DateTimeOffset.UtcNow,
            CustomerName = "Acme Corp"
        };
    }

    public async Task<IEnumerable<OrderLineDto>> GetOrderLinesAsync(Guid orderId, CancellationToken ct)
    {
        // var response = await http.GetAsync($"/orders/{orderId}/lines");
        // if (response.StatusCode == HttpStatusCode.NotFound)
        //     return [];

        // response.EnsureSuccessStatusCode();
        // var lines = await response.Content.ReadFromJsonAsync<IEnumerable<OrderLineDto>>();
        // return lines ?? [];

        await Task.Delay(100);

        return
        [
            new OrderLineDto
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                OrderId = orderId,
                Sku = "SKU-001",
                Quantity = 2,
                UnitPrice = 19.99m
            },
            new OrderLineDto
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                OrderId = orderId,
                Sku = "SKU-002",
                Quantity = 1,
                UnitPrice = 49.50m
            },
            new OrderLineDto
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                OrderId = orderId,
                Sku = "SKU-003",
                Quantity = 5,
                UnitPrice = 7.25m
            }
        ];
    }

    public async Task<PagedResponse<OrderOverview>> GetOrdersAsync(CancellationToken ct)
    {
        await Task.Delay(100);
        
        var order = new OrderOverview
        {
            OrderId = Guid.NewGuid().ToString(),
            OrderNumber = "ORD001",
            CustomerName = "Tech49",
            Amount = 12.5m,
            Status = OrderStatus.Paid,
            CreatedAt = DateTimeOffset.Now
        };

        return new PagedResponse<OrderOverview>
        {
            Page = 1,
            PageSize = 10,
            TotalItems = 1,
            TotalPages = 1,
            Items = [order]
        };
    }
}
