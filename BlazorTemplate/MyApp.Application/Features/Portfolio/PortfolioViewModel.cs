namespace MyApp.Application.Features.Portfolios;

using System.Reactive;
using MyApp.Application.Abstractions;

public sealed record LoadPortfolioCmd(Guid PortfolioId) : ICommand<Unit>;

public sealed class PortfolioViewModel : ViewModelBase
{

}
