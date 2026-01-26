namespace MyApp.Domain.Abstractions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public sealed class InvalidFormatException(string message) : DomainException(message);
