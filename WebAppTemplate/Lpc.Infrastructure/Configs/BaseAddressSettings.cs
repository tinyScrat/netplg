namespace Lpc.Infrastructure.Configs;

public sealed class BaseAddressSettings
{
    public const string SectionName = "Api";

    public string BaseAddress { get; init; } = default!;
}
