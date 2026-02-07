namespace MyApp.WebUI.Features.Auth;

using System.Reactive.Linq;
using MyApp.Application.Features.Auth;
using Microsoft.AspNetCore.Components.Authorization;

/// <summary>
/// Listen to the <see cref="AuthenticationStateProvider" /> and pupolated the Auth state store 
/// </summary>
/// <param name="logger"></param>
/// <param name="authProvider"></param>
/// <param name="store"></param>
public sealed class OidcAuthSyncEffect(
    ILogger<OidcAuthSyncEffect> logger,
    AuthenticationStateProvider authProvider,
    AuthStore store) : IDisposable
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
                    var user = state.User;

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
