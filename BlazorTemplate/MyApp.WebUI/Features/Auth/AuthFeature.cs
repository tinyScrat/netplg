namespace MyApp.Features.Auth;

using Microsoft.Extensions.DependencyInjection;

public static class AuthFeatures
{
    public static IServiceCollection AddUIAuthFeatures(
        this IServiceCollection services)
    {
        services.AddScoped<AuthDelegatingHandler>();
        services.AddSingleton<OidcAuthSyncEffect>();
        services.AddSingleton<SessionExpiredSubscriber>();

        return services;
    }
}

public static class UseAuthFeaturesExtensions
{
    public static IServiceProvider UseAuthFeatures(
        this IServiceProvider sp)
    {
        _ = sp.GetRequiredService<OidcAuthSyncEffect>();

        return sp;
    }
}
