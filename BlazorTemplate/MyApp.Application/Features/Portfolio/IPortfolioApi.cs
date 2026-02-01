namespace MyApp.Application.Features.Portfolios;

public sealed class PortfolioListViewDTO
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public double Amount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public interface IPortfolioApi
{
    Task<IEnumerable<PortfolioListViewDTO>> LoadPortfoliosAsync();
}
