namespace BlazorApp.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // APIs
        // services.AddSingleton<IAuthApi, AuthApi>();
        // services.AddSingleton<IPortfolioApi, PortfolioApi>();

        // // Auth helpers
        // services.AddSingleton<ITokenProvider, TokenProvider>();

        // // Browser storage
        // services.AddSingleton<IBrowserStorage, BrowserStorage>();

        return services;
    }
}
