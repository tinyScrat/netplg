namespace BlazorApp.Application.Features.Portfolios;

public interface IPortfolioStore
{
    PortfolioState Value { get; }
    IObservable<PortfolioState> Changes { get; }
}
