namespace BlazorApp.Application;

using Microsoft.Extensions.DependencyInjection;
using BlazorApp.Application.Features.Auth;
using BlazorApp.Application.Features.Portfolios;
using BlazorApp.Application.Features.Orders;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services
            .AddAuthFeature()
            .AddOrderFeature()
            .AddPortfolioFeature();

        return services;
    }
}
