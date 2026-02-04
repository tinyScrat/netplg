namespace MyApp.Application.Exceptions;

/// <summary>
/// Thrown when user is not authorized in UI layer
/// </summary>
/// <param name="inner"></param>
public sealed class AuthenticationUnavailableException(Exception inner)
    : Exception("Authentication is unavailable", inner) {}
