namespace Lpc.Domain.Abstractions;

using System;

/// <summary>
/// Base class for all Entities in DDD.
/// Equality is based on Id, not property values.
/// </summary>
/// <typeparam name="TKey">Type of the entity Id</typeparam>
public abstract class EntityBase<TId>
    where TId : EntityId<TId>
{
    protected TId Id { get; }

    protected EntityBase(TId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not EntityBase<TId> other) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(EntityBase<TId>? left, EntityBase<TId>? right)
        => ReferenceEquals(left, right) || (left is not null && right is not null && left.Equals(right));

    public static bool operator !=(EntityBase<TId>? left, EntityBase<TId>? right) => !(left == right);
}
