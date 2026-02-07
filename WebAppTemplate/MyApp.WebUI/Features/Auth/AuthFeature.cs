namespace MyApp.WebUI.Features.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

public static class AuthFeatures
{
    public static IServiceCollection AddBlazorAuthFeatures(
        this IServiceCollection services)
    {
        services.AddScoped<AuthDelegatingHandler>();
        services.AddSingleton<AuthStateChangedEventPublisher>();
        services.AddSingleton<SessionExpiredSubscriber>();

        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        return services;
    }
}

public static class UseAuthFeaturesExtensions
{
    public static IServiceProvider UseAuthFeatures(
        this IServiceProvider sp)
    {
        _ = sp.GetRequiredService<AuthStateChangedEventPublisher>();
        _ = sp.GetRequiredService<SessionExpiredSubscriber>();

        return sp;
    }
}
