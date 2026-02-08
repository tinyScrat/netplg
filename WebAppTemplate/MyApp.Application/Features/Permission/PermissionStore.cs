namespace MyApp.Application.Features.Permission;

using MyApp.Application.Abstractions;

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

    public void Reset() =>
        _state.Data.Update(p => PermissionState.Uninitialized);

    public void SetPermissions(IReadOnlySet<string> permissions) =>
        _state.Data.Update(p => new PermissionState(permissions, true));
}
