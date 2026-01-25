namespace MyApp.Domain.Abstractions;

using System.Globalization;

public interface IEntityIdentity
{
    string Key { get; }
}

/// <summary>
/// Base class for strongly-typed entity IDs with a prefix.
/// </summary>
/// <typeparam name="TDerived">Derived ID type, e.g., OrderId</typeparam>
public abstract class EntityId<TDerived> : IEquatable<TDerived>, IEntityIdentity
    where TDerived : EntityId<TDerived>
{
    private Guid Value { get; init; }

    /// <summary>
    /// Prefix to distinguish entity types (ord, cus, etc.)
    /// </summary>
    protected abstract string KeyPrefix { get; }

    protected EntityId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Create the string representation with prefix
    /// </summary>
    public string Key => string.Format(CultureInfo.InvariantCulture, "{0}_{1:N}", KeyPrefix, Value);

    public bool Equals(TDerived? other) => other is not null && Value.Equals(other.Value);

    public override bool Equals(object? obj) => obj is TDerived other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(EntityId<TDerived>? left, EntityId<TDerived>? right)
        => ReferenceEquals(left, right) || (left is not null && right is not null && left.Equals(right));

    public static bool operator !=(EntityId<TDerived>? left, EntityId<TDerived>? right) => !(left == right);

    public override string ToString() => Key;
}
