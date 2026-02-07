namespace MyApp.WebUI;

using MyApp.Application.Abstractions;
using MyApp.Application.Features.Storage;
using MyApp.WebUI.Features.Auth;
using MyApp.WebUI.Features.Events;
using MyApp.WebUI.Features.Storage;

internal static class Extensions
{
    public static IServiceCollection AddWebUIFeatures(
        this IServiceCollection services)
    {
        services.AddSingleton<BlazorAppEventBus>();
        services.AddSingleton<IAppEventBus>(sp =>
            sp.GetRequiredService<BlazorAppEventBus>());

        services.AddSingleton<IKeyValueStorage, BrowserLocalStorage>();

        services.AddBlazorAuthFeatures();

        return services;
    }
}
