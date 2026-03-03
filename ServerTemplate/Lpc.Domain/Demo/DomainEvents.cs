namespace Lpc.Domain;

using Lpc.Domain.Abstractions;

public sealed record CustomerAddressAdded(
    Guid Id,
    DateTimeOffset OccurredOnUtc,
    CustomerId CustomerId,
    Address Address
) : IDomainEvent;

public sealed record OrderPlaced(
    Guid Id,
    DateTimeOffset OccurredOnUtc,
    OrderId OrderId,
    CustomerId CustomerId
) : IDomainEvent;
