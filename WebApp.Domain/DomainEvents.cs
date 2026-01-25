namespace WebApp.Domain;

using WebApp.Domain.Abstractions;

public sealed record CustomerAddressAdded(
    CustomerId CustomerId,
    Address Address
) : DomainEvent;

public sealed record OrderPlaced(
    OrderId OrderId,
    CustomerId CustomerId
) : DomainEvent;
