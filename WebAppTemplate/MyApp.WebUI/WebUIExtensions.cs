namespace MyApp.WebUI;

using MyApp.Application.Services;
using MyApp.WebUI.Services;
using MyApp.WebUI.Layouts;
using MyApp.WebUI.Components;
using MyApp.WebUI.Features.Orders;
using MyApp.Application.Features.Portfolios;
using MyApp.WebUI.Features.Products;

internal static class WebUIExtensions
{
    public static IServiceCollection AddWebUIFeatures(
        this IServiceCollection services)
    {
        services.AddSingleton<IKeyValueStorage, BrowserLocalStorage>();

        services.AddScoped<AuthDelegatingHandler>();
        services.AddSingleton<AuthStateChangedEventPublisher>();
        services.AddSingleton<SessionExpiredSubscriber>();

        services.AddViewModels();

        return services;
    }

    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<HeaderViewModel>();
        services.AddTransient<MainMenuViewModel>();
        services.AddTransient<AuthorizeViewExViewModel>();

        services.AddTransient<EditOrderViewModel>();
        services.AddTransient<OrdersViewModel>();

        services.AddTransient<PortfolioViewModel>();
        services.AddTransient<ProductViewModel>();

        return services;
    }
}

public static class UseWebUIExtensions
{
    public static IServiceProvider UseWebUIFeatures(
        this IServiceProvider sp)
    {
        _ = sp.GetRequiredService<AuthStateChangedEventPublisher>();
        _ = sp.GetRequiredService<SessionExpiredSubscriber>();

        return sp;
    }
}
