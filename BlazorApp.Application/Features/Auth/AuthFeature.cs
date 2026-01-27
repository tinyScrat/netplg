namespace BlazorApp.Application.Features.Auth;

using Microsoft.Extensions.DependencyInjection;

public static class AuthFeature
{
    public static IServiceCollection AddAuthFeature(
        this IServiceCollection services)
    {
        // services.AddSingleton<AuthStore>();
        // services.AddSingleton<IAuthStore>(sp =>
        //     sp.GetRequiredService<AuthStore>());

        // services.AddSingleton<LoginEffect>();
        // services.AddSingleton<RefreshTokenEffect>();

        // services.AddTransient<LoginViewModel>();

        return services;
    }
}
