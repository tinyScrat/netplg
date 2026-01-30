using BlazorApp.Application.Abstractions;

namespace BlazorApp;

internal static class Extensions
{
    public static IServiceCollection AddBlazorAppFeatures(
        this IServiceCollection services)
    {
        services.AddSingleton<BlazorAppEventBus>();
        services.AddSingleton<IAppEventBus>(sp =>
            sp.GetRequiredService<BlazorAppEventBus>());


        services.AddSingleton<SessionExpiredSubscriber>();

        return services;
    }
}