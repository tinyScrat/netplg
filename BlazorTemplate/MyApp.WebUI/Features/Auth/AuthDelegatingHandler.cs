namespace MyApp.Features.Auth;

using MyApp.Application.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MyApp.Application.Exceptions;

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
        catch (AccessTokenNotAvailableException ex)
        {
            bus.Publish(new SessionExpiredEvent());
            
            // Translate UI exception → Application exception
            throw new AuthenticationUnavailableException(ex);
        }
    }
}
