namespace MyApp.Application.Features.Permission;

using System.Reactive.Linq;
using MyApp.Application.Abstractions;
using MyApp.Application.Features.Auth;

public sealed record LoadPermissionCommand() : ICommand<IReadOnlySet<string>>;

internal sealed class PermissionCommandKey(AuthStore authStore)
    : ICommandKey<LoadPermissionCommand>
{
    public string GetKey(LoadPermissionCommand _)
        => GetCurrentKey()
           ?? throw new InvalidOperationException("No authenticated user");

    public string? GetCurrentKey()
    {
        var userId = authStore.Current.UserName; // sub / oid
        return userId is null
            ? null
            : $"permissions:{userId}";
    }
}

internal sealed class AuthPermissionSyncSubscriber(
    IAppEventBus bus,
    IEnumerable<ICommandPipeline<LoadPermissionCommand>> pipelines,
    ICommandDispatcher dispatcher,
    PermissionStore permissionStore,
    ICommandKey<LoadPermissionCommand> keyProvider)
    : IDisposable
{
    private readonly IDisposable _sub =
        bus.OfType<AuthStateChangedEvent>()
            // Only react to changes in authentication state or user identity (e.g. user switch),
            // not to other changes like token refresh
            .DistinctUntilChanged(evt =>
                (
                    evt.User.Identity?.IsAuthenticated,
                    evt.User.Identity?.Name
                ))
            .Subscribe(evt =>
            {
                // Logout / user switch → cancel in-flight permission load
                if (evt.User.Identity?.IsAuthenticated != true)
                {
                    var key = keyProvider.GetCurrentKey();
                    if (key is not null)
                    {
                        foreach (var pipeline in pipelines)
                            pipeline.Cancel(key);
                    }

                    permissionStore.Reset();
                    return;
                }

                // Login / authenticated → load permissions
                dispatcher.Dispatch(new LoadPermissionCommand());
            });

    public void Dispose() => _sub.Dispose();
}
