namespace BlazorApp.Infrastructure;

using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;

public static class HttpObservableExtensions
{
    public static IObservable<T> WithAuthRedirect<T>(this IObservable<T> source)
    {
        return source
            .Catch<T, HttpRequestException>(ex =>
            {
                return Observable.Empty<T>();
            });
    }
}

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
