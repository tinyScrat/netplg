namespace MyApp.Domain;

using MyApp.Domain.Abstractions;

public abstract record EntityRef<TIdentity>
    where TIdentity : IEntityIdentity
{
    public TIdentity Id { get; }
    public string? DisplayName { get; }

    protected EntityRef(TIdentity id, string? displayName)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        DisplayName = displayName;
    }

    public override string ToString()
        => DisplayName ?? Id.Key;
}
