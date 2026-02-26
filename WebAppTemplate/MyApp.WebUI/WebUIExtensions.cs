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

        services
            .AddScoped<SessionExpiredSubscriber>()
            .AddScoped<AuthStateChangedEventPublisher>();

        services.AddViewModels();

        return services;
    }

    public static IServiceCollection AddApiHttpClientWithAuth(
        this IServiceCollection services,
        string httpClientName,
        IConfiguration configuration,
        string fallbackBaseAddress)
    {
        services
            .Configure<BaseAddressSettings>(configuration.GetSection(BaseAddressSettings.SectionName))
            .AddTransient<AuthDelegatingHandler>()
            .AddApiHttpClient(httpClientName, fallbackBaseAddress)
            // OUTERMOST: can catch exceptions from inner handlers (incl. AuthorizationMessageHandler)
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            // INNER: attaches access token for configured URLs
            .AddHttpMessageHandler(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("ApiHttpClient");

                var settings = sp.GetRequiredService<IOptions<BaseAddressSettings>>().Value;
                var uri = settings.ResolveBaseAddress(fallbackBaseAddress);

                logger.LogInformation("API BaseAddress: {BaseAddress}", uri);

                return sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler([uri.ToString()]);
            });

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
        return sp;
    }
}
