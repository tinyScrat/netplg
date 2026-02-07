namespace MyApp.Application.Features.Auth;

using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;

internal sealed class AuthStateChangedSubscriber(
    IAppEventBus eventBus,
    AuthStore store,
    ILogger<AuthStateChangedSubscriber> logger) : IDisposable
{
    private readonly IDisposable _subscription =
        eventBus.OfType<AuthStateChangedEvent>()
        .Subscribe(evt =>
        {
            var user = evt.User;
            logger.LogInformation("User {User} IsAuthenticated: {IsAuthenticated}",
                user.Identity?.Name, user.Identity?.IsAuthenticated);

            if (user.Identity?.IsAuthenticated == true)
            {
                store.SetAuthenticated(
                    user.Identity.Name ?? "Unknown",
                    user.FindAll("role")
                        .Select(r => r.Value));
            }
            else
            {
                store.SetAnonymous();
            }
        });

    public void Dispose() => _subscription.Dispose();
}
