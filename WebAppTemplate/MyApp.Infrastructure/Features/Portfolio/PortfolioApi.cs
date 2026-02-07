namespace MyApp.Infrastructure.Features.Portfolios;

using MyApp.Application.Features.Portfolios;
using Microsoft.Extensions.Logging;

internal sealed class PortfolioApi(ILogger<PortfolioApi> logger) : IPortfolioApi
{
    public Task<IEnumerable<PortfolioListViewDTO>> LoadPortfoliosAsync()
    {
        logger.LogInformation("Portfolios loaded");
        return Task.FromResult<IEnumerable<PortfolioListViewDTO>>([]);
    }
}
