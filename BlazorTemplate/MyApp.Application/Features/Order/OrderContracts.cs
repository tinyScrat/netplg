namespace MyApp.Contracts.Orders;

public sealed record OrderDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = default!;
    public DateTimeOffset CreatedAt { get; init; }
    public string CustomerName { get; init; } = default!;
}

public sealed record OrderLineDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string Sku { get; init; } = default!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public sealed record AddressDto(
    string Street,
    string City
);

public sealed record OrderItemDto(
    string ProductId,
    int Quantity
);

public sealed record OrderDraftDto(
    AddressDto Address,
    IReadOnlyList<OrderItemDto> Items
);

public enum OrderStatus
{
    New,
    Paid,
    Completed
}

public sealed record OrderOverview
{
    public string OrderId { get; init; } = default!;
    public string OrderNumber { get; init; } = default!;
    public string CustomerName { get; init; } = default!;
    public decimal Amount { get; init; }
    public OrderStatus Status { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
