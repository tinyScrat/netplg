namespace BlazorApp.Application.Features.Portfolio;

using Microsoft.Extensions.DependencyInjection;

public static class PortfolioFeature
{
    public static IServiceCollection AddPortfolioFeature(
        this IServiceCollection services)
    {
        // Store
        // services.AddSingleton<PortfolioStore>();
        // services.AddSingleton<IPortfolioStore>(sp =>
        //     sp.GetRequiredService<PortfolioStore>());

        // // Effects
        // services.AddSingleton<LoadPortfolioEffect>();

        // // ViewModels
        // services.AddTransient<PortfolioViewModel>();

        return services;
    }
}
