namespace Lpc.Infrastructure.Services;

using Lpc.Application.Features.Portfolios;
using Microsoft.Extensions.Logging;

internal sealed class PortfolioApi(
    HttpClient http,
    ILogger<PortfolioApi> logger) : IPortfolioApi
{
    public Task<IEnumerable<PortfolioListViewDTO>> LoadPortfoliosAsync()
    {
        _ = http;
        logger.LogInformation("Portfolios loaded");
        return Task.FromResult<IEnumerable<PortfolioListViewDTO>>([]);
    }
}
