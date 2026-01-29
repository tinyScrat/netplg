namespace BlazorApp.Application;

using Microsoft.Extensions.DependencyInjection;
using BlazorApp.Application.Features.Auth;
using BlazorApp.Application.Features.Portfolio;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services
            .AddAuthFeatures()
            .AddPortfolioFeature();

        return services;
    }
}
