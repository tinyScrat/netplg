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
