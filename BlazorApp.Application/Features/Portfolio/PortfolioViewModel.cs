namespace BlazorApp.Application.Features.Portfolios;

using BlazorApp.Application.Abstractions;

public sealed record LoadPortfolioCmd(Guid PortfolioId) : ICommand;

public sealed class PortfolioViewModel
{

}
