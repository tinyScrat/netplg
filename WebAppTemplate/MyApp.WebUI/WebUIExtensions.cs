namespace MyApp.WebUI;

using MyApp.Application.Features.Storage;
using MyApp.WebUI.Features.Auth;
using MyApp.WebUI.Features.Storage;

internal static class WebUIExtensions
{
    public static IServiceCollection AddWebUIFeatures(
        this IServiceCollection services)
    {
        services.AddSingleton<IKeyValueStorage, BrowserLocalStorage>();

        services.AddScoped<AuthDelegatingHandler>();
        services.AddSingleton<AuthStateChangedEventPublisher>();
        services.AddSingleton<SessionExpiredSubscriber>();

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
