namespace MyApp.Application.Features.Orders;

using MyApp.Contracts.Orders;

public sealed class Order
{
    public Guid Id { get; }
    public string OrderNumber { get; set; }
    public DateTimeOffset CreatedAt { get; }
    public string CustomerName { get; set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines;

    public Order(
        Guid id,
        string orderNumber,
        DateTimeOffset createdAt,
        string customerName,
        IEnumerable<OrderLine>? lines = null)
    {
        Id = id;
        OrderNumber = orderNumber;
        CreatedAt = createdAt;
        CustomerName = customerName;

        if (lines != null)
            _lines.AddRange(lines);
    }

    public void AddLine(OrderLine line)
    {
        if (line.OrderId != Id)
            throw new InvalidOperationException("OrderLine does not belong to this Order.");

        _lines.Add(line);
    }

    public void RemoveLine(Guid lineId)
    {
        _lines.RemoveAll(l => l.Id == lineId);
    }

    public decimal Total => _lines.Sum(l => l.LineTotal);
}


public sealed class OrderLine
{
    public Guid Id { get; }
    public Guid OrderId { get; }

    public string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;

    public OrderLine(
        Guid id,
        Guid orderId,
        string sku,
        int quantity,
        decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}



public interface IOrderApi
{
    Task SaveOrderAsync(OrderDraftDto dto);
    Task<OrderDto?> TryGetOrderAsync(Guid orderId, CancellationToken ct);
    Task<IEnumerable<OrderLineDto>> GetOrderLinesAsync(Guid orderId, CancellationToken ct);
}
