namespace MyApp.Infrastructure;

using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Storage;
using MyApp.Application.Features.Orders;
using MyApp.Application.Features.Products;
using MyApp.Application.Features.Portfolios;
using MyApp.Infrastructure.Features.Storage;
using MyApp.Infrastructure.Features.Orders;
using MyApp.Infrastructure.Features.Products;
using MyApp.Infrastructure.Features.Portfolios;

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
        services.AddSingleton<IOrderApi, OrderApi>();
        services.AddSingleton<IPortfolioApi, PortfolioApi>();
        services.AddSingleton<IProductApi, ProductApi>();

        // Browser storage
        services.AddSingleton<IBrowserStorage, BrowserStorage>();

        return services;
    }
}
