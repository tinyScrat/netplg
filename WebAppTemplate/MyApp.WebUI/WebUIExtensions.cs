namespace MyApp.WebUI;

using MyApp.Application.Services;
using MyApp.WebUI.Services;
using MyApp.WebUI.Layouts;
using MyApp.WebUI.Components;
using MyApp.WebUI.Features.Orders;
using MyApp.Application.Features.Portfolios;
using MyApp.WebUI.Features.Products;
using MyApp.WebUI.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MyApp.Infrastructure;
using Microsoft.Extensions.Options;
using MyApp.Infrastructure.Configs;

internal static class WebUIExtensions
{
    public static IServiceCollection AddWebUIFeatures(
        this IServiceCollection services)
    {
        services.AddSingleton<IKeyValueStorage, BrowserLocalStorage>();

        services.AddSingleton<AuthStateChangedEventPublisher>();
        services.AddSingleton<SessionExpiredSubscriber>();

        services.AddViewModels();

        return services;
    }

    public static IServiceCollection AddApiHttpClientWithAuth(
        this IServiceCollection services,
        string name,
        IConfiguration configuration,
        string fallbackBaseAddress)
    {
        services
            .Configure<BaseAddressSettings>(configuration.GetSection(BaseAddressSettings.SectionName))
            .AddTransient<AuthDelegatingHandler>()
            .AddApiHttpClient<HttpClient>(name, fallbackBaseAddress)
            .AddHttpMessageHandler(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<BaseAddressSettings>>().Value;
                var uri = ApiHttpClientExtensions.ResolveBaseAddress(settings, fallbackBaseAddress);

                return sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler(
                        authorizedUrls: [uri.ToString()]);
            })
            .AddHttpMessageHandler<AuthDelegatingHandler>();

        services
            .AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(name));

        return services;
    }

    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddScoped<GlobalErrorStore>();

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
