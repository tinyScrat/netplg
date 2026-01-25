namespace WebApp.Domain.Abstractions;

/*
    Characteristics of value object
    1. measure, quantify or describe a thing, but not a thing in itself
        A person is a thing, but name, age are attributes that describe the person

    2. Immunitable
        A value cannot be changed once created.

    3. Conceptual whole
        A value that describe money need to have amount and currency, these 2 attributes
        form a conceptual whole of monetary value

    4. Replaceablity
        Just like primitive value in a programming language, it got replaced with a new value
        that represent another value, not parts of it.

    5. Equality
        The object instance may not be the same, but as long as the attributes and the same,
        then these 2 value objects are the same.
*/

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base class for all Value Objects in DDD.
/// Implements structural equality and null-safe comparisons.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Derived classes must yield return all components used for equality comparison
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Null-safe Equals override for CS8765 and DDD equality
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        var thisComponents = GetEqualityComponents() ?? Enumerable.Empty<object?>();
        var otherComponents = other.GetEqualityComponents() ?? Enumerable.Empty<object?>();

        return thisComponents.SequenceEqual(otherComponents);
    }

    /// <summary>
    /// Null-safe and unchecked GetHashCode
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
