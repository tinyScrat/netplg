using MyApp.Application.Abstractions;

namespace MyApp.Application.Features.Portfolios;

public sealed class Portfolio
{
    public string Name { get; init; } = null!;
    public string Author { get; init; } = null!;
    public decimal Price { get; init; }
}

public sealed record PortfolioState(string Name, IEnumerable<Portfolio> Portfolios);

internal sealed class PortfolioStore(PortfolioState initial) : IPortfolioStore
{
    private ReactiveState<PortfolioState> _state = new(initial);

    public PortfolioState Value => _state.Value;

    public IObservable<PortfolioState> Changes => _state.Changes;

    public void UpdateName(string name) =>
        _state.Update(s => s with { Name = name });
}
