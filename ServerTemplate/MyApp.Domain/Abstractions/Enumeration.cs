namespace MyApp.Domain.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Enumeration : IComparable
{
    public string Name { get; private init; }
    public int Value { get; private init; }

    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration other) return false;
        return GetType() == obj.GetType() && Value == other.Value;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Value);

    public int CompareTo(object? obj)
    {
        if (obj is not Enumeration other) throw new ArgumentException("Object is not an Enumeration");
        return Value.CompareTo(other.Value);
    }

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T)
            .GetFields(System.Reflection.BindingFlags.Public | 
                       System.Reflection.BindingFlags.Static | 
                       System.Reflection.BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(x => x.Value == value);
        if (matchingItem == null)
            throw new ArgumentException($"No {typeof(T).Name} with Value {value}");
        return matchingItem;
    }

    public static T FromName<T>(string name) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(x => x.Name == name);
        if (matchingItem == null)
            throw new ArgumentException($"No {typeof(T).Name} with Name {name}");
        return matchingItem;
    }
}
