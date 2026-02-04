namespace MyApp.Domain.Abstractions;

using System.Net.Mail;

// Value Object implemented in modern C# using sealed record
// no abstract ValueObject base class

internal static class StringGuards
{
    public static string NotEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new EmptyValueException($"{name} is required");

        return value;
    }

    public static string MaxLength(string? value, int max, string name)
    {
        if (value != null && value.Length > max)
            throw new ArgumentException($"{name} exceeds {max} chars");
        return value ?? string.Empty;
    }
}

public sealed record Email
{
    public const int MaxLength = 256;

    public string Value { get; }

    public Email(string value)
    {
        var email = StringGuards.MaxLength(
            StringGuards.NotEmpty(value, nameof(Email)),
            MaxLength,
            nameof(Email));

        try
        {
            var mailAddress = new MailAddress(email);
            Value = mailAddress.Address; // normalized
        }
        catch (FormatException)
        {
            throw new InvalidFormatException($"Invalid email format: {value}");
        }
    }

    public override string ToString() => Value;
}

public sealed record Description
{
    public const int MaxLength = 256;

    public string Value { get; }

    public Description(string? value)
    {
        Value = StringGuards.MaxLength(
            StringGuards.NotEmpty(value, nameof(Description)),
            MaxLength,
            nameof(Description));
    }

    public override string ToString() => Value;
}

public sealed record Remark
{
    public const int MaxLength = 256;

    public string Value { get; }

    public Remark(string? value)
    {
        Value = StringGuards.MaxLength(value, MaxLength, nameof(Remark));
    }

    public override string ToString() => Value;
}
