namespace MyApp.Application.Abstractions;

using System.Security.Claims;

public abstract record AppEvent;

public sealed record SessionExpiredEvent : AppEvent;

public sealed record UserLoggedOutEvent : AppEvent;

public sealed record AuthStateChangedEvent(ClaimsPrincipal User) : AppEvent;
