namespace MyApp.Application.Features.Orders;

public interface IOrderApi
{
    Task SaveOrderAsync(OrderDraftDto dto);
}
