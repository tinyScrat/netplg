namespace MyApp.Domain.Primitives;

using System.Net.Mail;
using MyApp.Domain.Abstractions;

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
