namespace MyApp.WebUI.Features.Auth;

using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MyApp.Application.Abstractions;

public sealed class AuthStateChangedEventPublisher : IDisposable
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly NavigationManager _nav;
    private readonly ILogger<AuthStateChangedEventPublisher> _logger;
    private readonly IAppEventBus _eventBus;

    private bool _isFirstEmission = true;
    private bool? _wasAuthenticated; // null until first emission
    private readonly IDisposable _subscription;

    private static bool IsAuthenticated(AuthenticationState s)
        => s.User.Identity?.IsAuthenticated ?? false;

    private static bool IsAuthRoute(string baseRelativePath)
        => baseRelativePath.StartsWith("authentication/", StringComparison.OrdinalIgnoreCase);

    public AuthStateChangedEventPublisher(
        AuthenticationStateProvider authProvider,
        NavigationManager nav,
        ILogger<AuthStateChangedEventPublisher> logger,
        IAppEventBus eventBus)
    {
        _authProvider = authProvider;
        _nav = nav;
        _logger = logger;
        _eventBus = eventBus;

        _subscription =
            Observable
                .Defer(() => Observable.FromAsync(_authProvider.GetAuthenticationStateAsync))
                .Concat(
                    Observable
                        .FromEvent<AuthenticationStateChangedHandler, Task<AuthenticationState>>(
                            action => task => action(task),
                            h => _authProvider.AuthenticationStateChanged += h,
                            h => _authProvider.AuthenticationStateChanged -= h)
                        .SelectMany(task => task)
                )
                // Re-check once to avoid publishing transient unauthenticated around login callback.
                .SelectMany(async state =>
                {
                    if (!IsAuthenticated(state))
                    {
                        await Task.Delay(150).ConfigureAwait(false);

                        var refreshed = await _authProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
                        if (IsAuthenticated(refreshed))
                            return refreshed;
                    }

                    return state;
                })
                .Subscribe(state =>
                {
                    var isAuthenticated = IsAuthenticated(state);
                    var current = _nav.ToBaseRelativePath(_nav.Uri);
                    var inAuthFlow = IsAuthRoute(current);

                    // During /authentication/* (login-callback, etc.) the provider may briefly be unauthenticated.
                    // Ignore that to avoid resetting app state or triggering redirects too early.
                    if (inAuthFlow && !isAuthenticated)
                    {
                        _logger.LogDebug("Ignoring unauthenticated auth-state while in auth flow. Route={Route}", current);
                        _isFirstEmission = false;
                        _wasAuthenticated = false;
                        return;
                    }

                    _logger.LogInformation(
                        "Authentication state changed, user: {User}",
                        state.User.Identity?.Name ?? "anonymous");

                    _eventBus.Publish(new AuthStateChangedEvent(state.User));

                    if (_isFirstEmission)
                    {
                        if (!isAuthenticated)
                            _eventBus.Publish(new SessionExpiredEvent());

                        _isFirstEmission = false;
                    }
                    else if (_wasAuthenticated == true && !isAuthenticated)
                    {
                        _eventBus.Publish(new SessionExpiredEvent());
                    }

                    _wasAuthenticated = isAuthenticated;
                });
    }

    public void Dispose() => _subscription.Dispose();
}
