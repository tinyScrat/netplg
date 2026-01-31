namespace MyApp.Features.Auth;

using MyApp.Application.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

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
        catch (AccessTokenNotAvailableException)
        {
            bus.Publish(new SessionExpiredEvent());
            throw;
        }
    }
}
