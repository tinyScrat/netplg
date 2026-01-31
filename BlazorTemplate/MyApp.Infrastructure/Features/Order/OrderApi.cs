namespace MyApp.Infrastructure.Features.Orders;

using System.Threading.Tasks;
using MyApp.Application.Features.Orders;

internal sealed class OrderApi : IOrderApi
{
    public Task SaveOrderAsync(OrderDraftDto dto)
    {
        return Task.CompletedTask;
    }
}
