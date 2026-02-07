namespace MyApp.Application.Features.Permission;

public interface IPermissionStore
{
    bool IsInitialized { get; }
    IReadOnlySet<string> Permissions { get; }
}
