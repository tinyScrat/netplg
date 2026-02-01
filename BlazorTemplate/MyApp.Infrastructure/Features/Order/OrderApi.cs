namespace MyApp.Infrastructure.Features.Orders;

using System.Threading.Tasks;
using MyApp.Application.Features.Orders;
using Microsoft.Extensions.Logging;

internal sealed class OrderApi(ILogger<OrderApi> logger) : IOrderApi
{
    public Task SaveOrderAsync(OrderDraftDto dto)
    {
        logger.LogInformation("Order saved");
        return Task.CompletedTask;
    }
}
