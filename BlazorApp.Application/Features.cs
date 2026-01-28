namespace BlazorApp.Application;

using Microsoft.Extensions.DependencyInjection;
using BlazorApp.Application.Features.Auth;
using BlazorApp.Application.Features.Portfolio;
using BlazorApp.Application.Features.App;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services
            .AddAppFeature()
            .AddAuthFeatures()
            .AddPortfolioFeature();

        return services;
    }
}
