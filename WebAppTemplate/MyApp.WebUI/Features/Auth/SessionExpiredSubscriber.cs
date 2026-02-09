namespace MyApp.WebUI.Features.Auth;

using System.Reactive.Linq;
using MyApp.Application.Abstractions;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Listen to <see cref="SessionExpiredEvent" /> and redirect user to login page
/// </summary>
/// <param name="bus"></param>
/// <param name="nav"></param>
public sealed class SessionExpiredSubscriber(
    ILogger<SessionExpiredSubscriber> logger,
    IAppEventBus bus,
    NavigationManager nav) : IDisposable
{
    private const string LoginPath = "authentication/login";
    private readonly IDisposable _sub = bus
            .OfType<SessionExpiredEvent>()
            //.Take(1) // idempotency
            .Subscribe(_ =>
            {
                // If we're already on the login page, don't re-navigate.
                var current = nav.ToBaseRelativePath(nav.Uri);
                if (current.StartsWith(LoginPath, StringComparison.OrdinalIgnoreCase)) return;

                logger.LogInformation("Session expired (or unauthenticated at startup), redirecting to login page");

                // Optional: preserve returnUrl
                var returnUrl = Uri.EscapeDataString(current);
                nav.NavigateTo($"{LoginPath}?returnUrl={returnUrl}", replace: true);
            });

    public void Dispose() => _sub.Dispose();
}
