namespace MyApp.WebUI.Features.Auth;

using System.Reactive.Linq;
using Microsoft.AspNetCore.Components.Authorization;
using MyApp.Application.Abstractions;

/// <summary>
/// Listen to the <see cref="AuthenticationStateProvider" /> and pupolated the Auth state store 
/// </summary>
/// <param name="logger"></param>
/// <param name="authProvider"></param>
/// <param name="store"></param>
public sealed class AuthStateChangedEventPublisher(
    AuthenticationStateProvider authProvider,
    ILogger<AuthStateChangedEventPublisher> logger,
    IAppEventBus eventBus) : IDisposable
{
    private readonly IDisposable _subscription =
            Observable
                // 1) Seed store from the current auth state at startup
                .Defer(() => Observable.FromAsync(authProvider.GetAuthenticationStateAsync))
                // 2) Then apply subsequent changes
                .Concat(
                    Observable
                        .FromEvent<AuthenticationStateChangedHandler, Task<AuthenticationState>>(
                            action => task => action(task),
                            h => authProvider.AuthenticationStateChanged += h,
                            h => authProvider.AuthenticationStateChanged -= h)
                        .SelectMany(task => task)
                )
                .Subscribe(state =>
                {
                    logger.LogInformation("Authentication state changed, user: {User}", state.User.Identity?.Name ?? "anonymous");
                    // Broadcast event to Application layer
                    eventBus.Publish(new AuthStateChangedEvent(state.User));
                });

    public void Dispose() => _subscription.Dispose();
}
