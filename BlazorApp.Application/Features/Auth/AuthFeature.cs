namespace BlazorApp.Application.Features.Auth;

using Microsoft.Extensions.DependencyInjection;

public static class AuthFeatures
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddSingleton<AuthStore>();
        // services.AddSingleton<IAuthStore>(sp =>
        //     sp.GetRequiredService<AuthStore>());

        services.AddTransient<AuthViewModel>();

        return services;
    }
}
