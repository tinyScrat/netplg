using MyApp.Application.Abstractions;

namespace MyApp.Application.Features.Permission;

public sealed record PermissionState(
    IReadOnlySet<string> Permissions,
    bool IsInitialized)
{
    public static PermissionState Uninitialized =>
        new(new HashSet<string>(), false);
}

public sealed class PermissionStore : IPermissionStore
{
    private readonly AsyncState<PermissionState> _state =
        new(PermissionState.Uninitialized);

    public bool IsInitialized => _state.Data.Value.IsInitialized;
    public IReadOnlySet<string> Permissions => _state.Data.Value.Permissions;
}
