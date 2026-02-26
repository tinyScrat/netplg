namespace MyApp.Application;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Portfolios;
using MyApp.Application.Features.Orders;
using MyApp.Application.Features.Products;
using MyApp.Application.Abstractions;
using MyApp.Application.Features.User;
using MyApp.Application.Services;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddSingleton<AppEventBus>();
        services.AddSingleton<IAppEventBus>(sp =>
            sp.GetRequiredService<AppEventBus>());

        // Safe as singleton now: dispatcher only resolves ICommandPipeline<T>,
        // which are also singletons (they create their own scopes internally).
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        services
            .AddOrderFeature()
            .AddProductFeature()
            .AddPortfolioFeature()
            .AddUserProfileFeature();

        return services;
    }

    public static IServiceProvider UseApplicationFeatures(this IServiceProvider sp)
    {
        // Force the DI container to create the effects instance and its dependencies.
        sp.UseUserProfileFeature();

        return sp;
    }
}
