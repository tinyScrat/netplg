namespace MyApp.Domain.Abstractions;

using System.Net.Mail;

internal static class StringGuards
{
    public static string MaxLength(string value, int max, string name)
    {
        if (value != null && value.Length > max)
            throw new ArgumentException($"{name} exceeds {max} chars");
        return value ?? string.Empty;
    }
}

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required", nameof(value));

        try
        {
            var mailAddress = new MailAddress(value);
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
    public string Value { get; }

    public Description(string value)
    {
        Value = StringGuards.MaxLength(value, 256, nameof(Description));
    }
}
