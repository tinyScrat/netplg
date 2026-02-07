namespace MyApp.Application;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Auth;
using MyApp.Application.Features.Portfolios;
using MyApp.Application.Features.Orders;
using MyApp.Application.Features.Products;
using MyApp.Application.Features.Permission;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services
            .AddAuthFeature()
            .AddPermissionFeature()
            .AddOrderFeature()
            .AddProductFeature()
            .AddPortfolioFeature();

        return services;
    }
}

public static class UseApplicationExtensions
{
    public static IServiceProvider UseApplicationFeatures(this IServiceProvider sp)
    {
        _ = sp.GetRequiredService<AuthStateChangedSubscriber>();

        return sp;
    }
}
