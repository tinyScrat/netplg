namespace BlazorApp.Features.Auth;

using System.Reactive;
using System.Reactive.Linq;
using BlazorApp.Application.Features.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public static class UiAuthObservableExtensions
{
    public static IObservable<T> WithAuthRedirect<T>(
        this IObservable<T> source,
        NavigationManager nav)
    {
        return source.Catch<T, AccessTokenNotAvailableException>(ex =>
        {
            ex.Redirect();
            return Observable.Empty<T>();
        });
    }
}


public sealed class OidcAuthSyncEffect : IDisposable
{
    private readonly IDisposable _subscription;

    public OidcAuthSyncEffect(
        AuthenticationStateProvider authProvider,
        AuthStore store)
    {
        _subscription =
            Observable
                .FromEventPattern<AuthenticationStateChangedHandler,
                    Task<AuthenticationState>>(
                    h => authProvider.AuthenticationStateChanged += h,
                    h => authProvider.AuthenticationStateChanged -= h)
                .StartWith(default(EventPattern<Task<AuthenticationState>>))
                .SelectMany(_ => authProvider.GetAuthenticationStateAsync())
                .Subscribe(state =>
                {
                    var user = state.User;

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
    }

    public void Dispose() => _subscription.Dispose();
}
