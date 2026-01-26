namespace MyApp.Domain.Abstractions;

using System.Globalization;

public interface IEntityIdentity
{
    string Key { get; }
}

public abstract record EntityId<TDerived> : IEntityIdentity
    where TDerived : EntityId<TDerived>
{
    protected internal Guid Value { get; }

    protected abstract string KeyPrefix { get; }

    protected EntityId(Guid value)
    {
        if (value == Guid.Empty)
            throw new EmptyValueException($"{GetType().Name} cannot be empty");

        Value = value;
    }

    public string Key =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"{KeyPrefix}_{Value:N}");

    public override string ToString() => Key;
}
