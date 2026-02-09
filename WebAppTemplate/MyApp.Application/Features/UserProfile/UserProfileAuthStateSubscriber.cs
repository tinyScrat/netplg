namespace MyApp.Application.Features.User;

using System.Reactive.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;

public sealed record LoadUserProfileCmd(ClaimsPrincipal User) : ICommand<UserProfile>;

internal sealed class LoadUserProfileCmdKey
    : ICommandKey<LoadUserProfileCmd>
{
    public string GetKey(LoadUserProfileCmd cmd)
    {
        var userId = cmd.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? cmd.User.Identity?.Name;
        
        return userId is null
            ? throw new InvalidOperationException("No authenticated user")
            : $"userprofile:{userId}";
    }

    public string? GetCurrentKey()
    {
        // This method is used during logout, so we may not have the current principal
        return null;
    }
}

internal sealed class UserProfileAuthStateSubscriber(
    ILogger<UserProfileAuthStateSubscriber> logger,
    IAppEventBus bus,
    IEnumerable<ICommandPipeline<LoadUserProfileCmd>> pipelines,
    ICommandDispatcher dispatcher,
    UserProfileStore userProfileStore,
    ICommandKey<LoadUserProfileCmd> keyProvider)
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
                logger.LogInformation("Auth state changed. User: {User}, IsAuthenticated: {IsAuthenticated}",
                    evt.User.Identity?.Name, evt.User.Identity?.IsAuthenticated);

                // Logout / user switch → cancel in-flight permission load
                if (evt.User.Identity?.IsAuthenticated != true)
                {
                    var key = keyProvider.GetCurrentKey();
                    if (key is not null)
                    {
                        foreach (var pipeline in pipelines)
                            pipeline.Cancel(key);
                    }

                    userProfileStore.SetUnauthenticated();
                    return;
                }

                // Login / authenticated → load permissions
                logger.LogInformation("Dispatching LoadUserProfileCmd for user {User}", evt.User.Identity.Name);
                dispatcher.Dispatch(new LoadUserProfileCmd(evt.User));
            });

    public void Dispose() => _sub.Dispose();
}
