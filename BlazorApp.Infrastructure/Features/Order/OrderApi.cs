namespace BlazorApp.Infrastructure.Features.Orders;

using System.Threading.Tasks;
using BlazorApp.Application.Features.Orders;

internal sealed class OrderApi : IOrderApi
{
    public Task SaveOrderAsync(OrderDraftDto dto)
    {
        return Task.CompletedTask;
    }
}
