namespace BlazorApp;

using System.Reactive.Linq;
using BlazorApp.Application.Abstractions;
using Microsoft.AspNetCore.Components;

public sealed class SessionExpiredSubscriber(
    BlazorAppEventBus bus,
    NavigationManager nav) : IDisposable
{
   private readonly IDisposable _sub = bus
           .OfType<SessionExpiredEvent>()
           .Take(1) // idempotency
           .Subscribe(_ =>
           {
               nav.NavigateTo("authentication/login", forceLoad: true);
           });

    public void Dispose() => _sub.Dispose();
}
