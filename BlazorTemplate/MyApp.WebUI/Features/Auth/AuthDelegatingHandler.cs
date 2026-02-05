namespace MyApp.Features.Auth;

using MyApp.Application.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MyApp.Application.Exceptions;

/// <summary>
/// Responsible for broadcasting to the whole system that the access token
/// is not longer valid after trying to refresh, should force user to the
/// login page
/// </summary>
/// <param name="bus"></param>
public sealed class AuthDelegatingHandler(IAppEventBus bus) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request,
       CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (AccessTokenNotAvailableException ex) // the framework provided oidc lib has tried token refresh but failed at this point
        {
            bus.Publish(new SessionExpiredEvent());
            
            // Translate UI exception → Application exception
            throw new AuthenticationUnavailableException(ex);
        }
    }
}
