namespace MyApp;

using MyApp.Application.Abstractions;
using MyApp.Features.Auth;
using MyApp.Features.Events;


internal static class Extensions
{
    public static IServiceCollection AddBlazorAppFeatures(
        this IServiceCollection services)
    {
        services.AddSingleton<BlazorAppEventBus>();
        services.AddSingleton<IAppEventBus>(sp =>
            sp.GetRequiredService<BlazorAppEventBus>());

        services.AddUIAuthFeatures();

        return services;
    }
}
