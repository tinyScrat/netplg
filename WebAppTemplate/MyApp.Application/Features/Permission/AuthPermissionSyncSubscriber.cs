namespace MyApp.Application.Features.Permission;

using System.Reactive.Linq;
using MyApp.Application.Abstractions;
using MyApp.Application.Features.Auth;

public sealed record LoadPermissionCommand() : ICommand<IReadOnlySet<string>>;

internal sealed class PermissionCommandKey(AuthStore authStore)
    : ICommandKey<LoadPermissionCommand>
{
    public string GetKey(LoadPermissionCommand _)
        => $"permissions:{authStore.Current.UserName ?? "anonymous"}";
}

internal sealed class AuthPermissionSyncSubscriber(
    IAppEventBus bus,
    ICommandDispatcher dispatcher,
    PermissionStore permissionStore) : IDisposable
{
    private readonly IDisposable _sub =
            bus.OfType<AuthStateChangedEvent>()
                .Subscribe(evt =>
                {
                    if (evt.User.Identity?.IsAuthenticated == true)
                    {
                        dispatcher.Dispatch(new LoadPermissionCommand());
                    }
                    else
                    {
                        permissionStore.Reset();
                    }
                });

    public void Dispose() => _sub.Dispose();
}
