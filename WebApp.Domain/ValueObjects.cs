namespace WebApp.Domain;

using WebApp.Domain.Abstractions;

public sealed class Money : ValueObject
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;

    // EF Core parameterless constructor
    private Money() { }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

public sealed class Address : ValueObject
{
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string ZipCode { get; init; } = null!;

    private Address() { }

    public Address(string street, string city, string zipCode)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return ZipCode;
    }
}

public sealed class Name : ValueObject
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;

    private Name() { }

    public Name(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
