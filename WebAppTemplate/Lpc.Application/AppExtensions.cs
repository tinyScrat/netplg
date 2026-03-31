namespace Lpc.Application;

using Microsoft.Extensions.DependencyInjection;
using Lpc.Application.Features.Portfolios;
using Lpc.Application.Features.Orders;
using Lpc.Application.Features.Products;
using Lpc.Application.Abstractions;
using Lpc.Application.Features.User;
using Lpc.Application.Services;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddSingleton<AppEventBus>();
        services.AddSingleton<IAppEventBus>(sp =>
            sp.GetRequiredService<AppEventBus>());

        services.AddScoped<GlobalErrorStore>();

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
