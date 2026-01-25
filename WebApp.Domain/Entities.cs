namespace WebApp.Domain;

using WebApp.Domain.Abstractions;
using System.Collections.Generic;

public sealed record CustomerRef : EntityRef<CustomerId>
{
    public CustomerRef(CustomerId id, string? displayName)
        : base(id, displayName)
    {
    }
}

public class Order : AggregateRoot<OrderId>
{
    public OrderId OrderId => Id;

    public Money Total { get; private set; }
    public CustomerId CustomerId { get; private set; } = null!;

    private readonly List<Address> _shippingAddresses = new();
    public IReadOnlyCollection<Address> ShippingAddresses => _shippingAddresses.AsReadOnly();

    public Order(OrderId id, CustomerId customerId, Money total) : base(id)
    {
        Total = total ?? throw new ArgumentNullException(nameof(total));

        RaiseDomainEvent(new OrderPlaced(OrderId, CustomerId));
    }

    public void AddAddress(Address address)
    {
        _shippingAddresses.Add(address ?? throw new ArgumentNullException(nameof(address)));
    }
}

public class Customer : AggregateRoot<CustomerId>
{
    public CustomerId CustomerId => Id;

    public Name FullName { get; private set; }

    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    public Customer(CustomerId id, Name fullName) : base(id)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
    }

    public void ChangeName(Name newName)
    {
        FullName = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void AddAddress(Address address)
    {
        if (address is null)
            throw new ArgumentNullException(nameof(address));

        if (_addresses.Any(a => a == address))
            throw new DuplicateAddressException();

        _addresses.Add(address);

        RaiseDomainEvent(new CustomerAddressAdded(CustomerId, address));
    }

    public void RemoveAddress(Address address)
    {
        if (address is null)
            throw new ArgumentNullException(nameof(address));

        if (!_addresses.Remove(address))
            throw new AddressNotFoundException();

    }
}
