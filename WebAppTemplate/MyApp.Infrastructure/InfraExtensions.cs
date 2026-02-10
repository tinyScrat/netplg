namespace MyApp.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Orders;
using MyApp.Application.Features.Products;
using MyApp.Application.Features.Portfolios;
using MyApp.Application.Features.User;
using MyApp.Infrastructure.Services;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IOrderApi, OrderApi>();
        services.AddSingleton<IPortfolioApi, PortfolioApi>();
        services.AddSingleton<IProductApi, ProductApi>();
        services.AddSingleton<IUserProfileApi, UserProfileApi>();

        return services;
    }
}
