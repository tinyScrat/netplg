namespace MyApp.Features.Auth;

using System.Reactive.Linq;
using MyApp.Application.Features.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public static class UiAuthObservableExtensions
{
    public static IObservable<T> WithAuthRedirect<T>(
        this IObservable<T> source)
    {
        return source.Catch<T, AccessTokenNotAvailableException>(ex =>
        {
            ex.Redirect();
            return Observable.Empty<T>();
        });
    }
}


public sealed class OidcAuthSyncEffect(
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

                    Console.WriteLine($"OIDC Auth State Changed. IsAuthenticated: {user.Identity?.IsAuthenticated}");

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
