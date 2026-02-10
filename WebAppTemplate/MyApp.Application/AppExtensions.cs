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

        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        services
            .AddOrderFeature()
            .AddProductFeature()
            .AddPortfolioFeature()
            .AddUserProfileFeature();

        return services;
    }
}

public static class UseApplicationExtensions
{
    public static IServiceProvider UseApplicationFeatures(this IServiceProvider sp)
    {
        _ = sp.GetRequiredService<UserProfileAuthStateSubscriber>();

        return sp;
    }
}
