namespace MyApp.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Features.Orders;
using MyApp.Application.Features.Products;
using MyApp.Application.Features.Portfolios;
using MyApp.Application.Features.User;
using MyApp.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Mime;
using MyApp.Infrastructure.Configs;
using Microsoft.Extensions.Options;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string httpClientName)
    {
        services.AddHttpClient<IOrderApi, OrderApi>(httpClientName);
        services.AddHttpClient<IPortfolioApi, PortfolioApi>(httpClientName);
        services.AddHttpClient<IProductApi, ProductApi>(httpClientName);
        services.AddHttpClient<IUserProfileApi, UserProfileApi>(httpClientName);

        return services;
    }

    public static IHttpClientBuilder AddApiHttpClient<TClient>(
        this IServiceCollection services,
        string httpClientName,
        string fallbackBaseAddress) where TClient : class
    {
        return services
            .AddHttpClient(httpClientName, (sp, client) =>
            {
                var logger = sp.GetRequiredService<ILogger<TClient>>();

                var settings = sp.GetRequiredService<IOptions<BaseAddressSettings>>().Value;
                var uri = ApiHttpClientExtensions.ResolveBaseAddress(settings, fallbackBaseAddress);

                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                logger.LogInformation(
                    "Configured API client {Client} with BaseAddress {BaseAddress}",
                    typeof(TClient).Name,
                    uri);
            });
    }
}

public static class ApiHttpClientExtensions
{
    public static Uri ResolveBaseAddress(
        BaseAddressSettings settings,
        string fallbackBaseAddress)
    {
        var configured = settings.BaseAddress;

        var baseAddress = string.IsNullOrWhiteSpace(configured)
            ? fallbackBaseAddress
            : configured;

        return Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri)
            ? uri
            : new Uri(fallbackBaseAddress);
    }
}
