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
   private readonly IDisposable _sub = bus
           .OfType<SessionExpiredEvent>()
           .Take(1) // idempotency
           .Subscribe(_ =>
           {
                logger.LogInformation("Session expired, redirecting to login page");
                nav.NavigateTo("authentication/login", forceLoad: true);
           });

    public void Dispose() => _sub.Dispose();
}
