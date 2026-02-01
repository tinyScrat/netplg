namespace MyApp.Application;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Auth;
using MyApp.Application.Features.Portfolios;
using MyApp.Application.Features.Orders;
using MyApp.Application.Features.Products;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services
            .AddAuthFeature()
            .AddOrderFeature()
            .AddProductFeature()
            .AddPortfolioFeature();

        return services;
    }
}
