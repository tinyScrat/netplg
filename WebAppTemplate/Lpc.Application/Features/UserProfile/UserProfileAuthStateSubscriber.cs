namespace Lpc.Application.Features.User;

using System.Reactive.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Lpc.Application.Abstractions;

/// <summary>
/// Subscriber that listens to authentication state changes and triggers user profile loading or clearing accordingly.
/// </summary>
/// <param name="logger"></param>
/// <param name="bus"></param>
/// <param name="dispatcher"></param>
/// <param name="userProfileStore"></param>
internal sealed class UserProfileAuthStateSubscriber(
    ILogger<UserProfileAuthStateSubscriber> logger,
    IAppEventBus bus,
    ICommandDispatcher dispatcher,
    UserProfileStore userProfileStore) : IDisposable
{
    private readonly IDisposable sub =
        bus.OfType<AuthStateChangedEvent>()
            // Only react to changes in authentication state or user identity (e.g. user switch),
            // not to other changes like token refresh
            .DistinctUntilChanged(evt =>
                (
                    evt.User.Identity?.IsAuthenticated,
                    evt.User.Identity?.Name,
                    evt.User?.FindFirst("sub")?.Value,
                    evt.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ))
            .Subscribe(
                evt =>
                {
                    logger.LogInformation(
                        "Auth state changed. User: {User}, IsAuthenticated: {IsAuthenticated}",
                        evt.User.Identity?.Name, evt.User.Identity?.IsAuthenticated);

                    // Logout / user switch
                    if (evt.User.Identity?.IsAuthenticated != true)
                    {
                        userProfileStore.SetUnauthenticated();
                        return;
                    }

                    // Login / authenticated → load profile
                    dispatcher.Dispatch(new LoadUserProfileCmd(evt.User));
                },
                ex => logger.LogError(ex, "Unhandled error in auth state subscriber."));

    public void Dispose() => sub.Dispose();
}
