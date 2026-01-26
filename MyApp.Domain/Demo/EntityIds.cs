namespace MyApp.Domain;

using MyApp.Domain.Abstractions;

public sealed class OrderId : EntityId<OrderId>
{
    public OrderId(Guid value) : base(value) { }
    public static OrderId New() => new(Guid.NewGuid());
    protected override string KeyPrefix => "ord";
}

public sealed class CustomerId : EntityId<CustomerId>
{
    public CustomerId(Guid value) : base(value) { }
    public static CustomerId New() => new(Guid.NewGuid());
    protected override string KeyPrefix => "cus";
}
