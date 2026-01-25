namespace WebApp.Domain;

using WebApp.Domain.Abstractions;

public sealed class ConcurrencyException : DomainException
{
    public ConcurrencyException(string message) : base(message) { }
}

public sealed class DuplicateAddressException : DomainException
{
    public DuplicateAddressException()
        : base("The address already exists for this customer.")
    {
    }
}

public sealed class AddressNotFoundException : DomainException
{
    public AddressNotFoundException()
        : base("The address does not exist for this customer.")
    {
    }
}
