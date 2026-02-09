namespace MyApp.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Orders;
using MyApp.Application.Features.Products;
using MyApp.Application.Features.Portfolios;
using MyApp.Infrastructure.Features.Orders;
using MyApp.Infrastructure.Features.Products;
using MyApp.Infrastructure.Features.Portfolios;
using MyApp.Application.Features.Auth;
using MyApp.Infrastructure.Features.Auth;
using MyApp.Infrastructure.Features.User;
using MyApp.Application.Features.User;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAuthApi, AuthApi>();
        services.AddSingleton<IOrderApi, OrderApi>();
        services.AddSingleton<IPortfolioApi, PortfolioApi>();
        services.AddSingleton<IProductApi, ProductApi>();
        services.AddSingleton<IUserProfileApi, UserProfileApi>();

        return services;
    }
}
