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

    public static IHttpClientBuilder AddApiHttpClient<TClient>(
        this IServiceCollection services,
        string name,
        IConfiguration configuration,
        string fallbackBaseAddress) where TClient : class
    {
        services
            .AddScoped(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return factory.CreateClient(name);
            });

        return services
            .AddHttpClient(name, (sp, client) =>
            {
                var logger = sp.GetRequiredService<ILogger<TClient>>();

                var configuredBaseAddress = configuration.GetValue<string>("BaseAddress");

                var baseAddress =
                    string.IsNullOrWhiteSpace(configuredBaseAddress)
                        ? fallbackBaseAddress
                        : configuredBaseAddress;

                if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri))
                {
                    logger.LogWarning(
                        "Invalid BaseAddress '{BaseAddress}', falling back to '{Fallback}'",
                        baseAddress,
                        fallbackBaseAddress);

                    uri = new Uri(fallbackBaseAddress);
                }

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
