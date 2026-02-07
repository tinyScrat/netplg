namespace MyApp.Application.Features.Auth;

using Microsoft.Extensions.DependencyInjection;

public static class AuthFeatures
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddSingleton<AuthStore>();
        services.AddSingleton<AuthStateChangedSubscriber>();

        services.AddTransient<AuthViewModel>();

        return services;
    }
}
