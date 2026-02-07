namespace MyApp.Application.Features.Portfolios;

using Microsoft.Extensions.DependencyInjection;

public static class PortfolioFeature
{
    public static IServiceCollection AddPortfolioFeature(
        this IServiceCollection services)
    {
        // One concrete state store, one instance.
        // Multiple consumers see it through different “roles”.
        services.AddSingleton<PortfolioStore>();
        services.AddSingleton<IPortfolioStore>(sp =>
            sp.GetRequiredService<PortfolioStore>());

        // // Effects
        // services.AddSingleton<LoadPortfolioEffect>();

        // // ViewModels
        // services.AddTransient<PortfolioViewModel>();

        return services;
    }
}
