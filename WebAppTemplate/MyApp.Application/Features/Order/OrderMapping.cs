namespace MyApp.Application.Features.Orders;

using MyApp.Contracts.Orders;

internal static class OrderMappings
{
    public static Order ToModel(this OrderDto dto)
        => new(
            id: dto.Id,
            orderNumber: dto.OrderNumber,
            createdAt: dto.CreatedAt,
            customerName: dto.CustomerName
        );

    public static OrderLine ToModel(this OrderLineDto dto)
        => new(
            id: dto.Id,
            orderId: dto.OrderId,
            sku: dto.Sku,
            quantity: dto.Quantity,
            unitPrice: dto.UnitPrice
        );

    public static Order Compose(OrderDto orderDto, IEnumerable<OrderLineDto> lineDtos)
    {
        var order = orderDto.ToModel();
        foreach (var line in lineDtos)
            order.AddLine(line.ToModel());
        return order;
    }
}
