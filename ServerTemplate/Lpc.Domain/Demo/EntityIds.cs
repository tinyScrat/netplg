namespace Lpc.Domain;

using Lpc.Domain.Abstractions;

public sealed record OrderId : EntityId<OrderId>
{
    public OrderId(Guid value) : base(value) { }
    public static OrderId New() => new(Guid.NewGuid());
    protected override string KeyPrefix => "ord";
}

public sealed record CustomerId : EntityId<CustomerId>
{
    public CustomerId(Guid value) : base(value) { }
    public static CustomerId New() => new(Guid.NewGuid());
    protected override string KeyPrefix => "cus";
}
