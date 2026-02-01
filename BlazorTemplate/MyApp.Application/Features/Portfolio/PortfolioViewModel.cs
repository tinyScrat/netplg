namespace MyApp.Application.Features.Portfolios;

using MyApp.Application.Abstractions;

public sealed record LoadPortfolioCmd(Guid PortfolioId) : ICommand;

public sealed class PortfolioViewModel : ReactiveViewModel<ICommand>
{

}
