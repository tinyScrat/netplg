using System.Net;
using BlazorApp.Application.Abstractions;

namespace BlazorApp.Infrastructure;

public sealed class AuthDelegatingHandler(IAppEventBus bus) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request,
       CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            bus.Publish(new SessionExpiredEvent());
        }

        return response;
    }
}
