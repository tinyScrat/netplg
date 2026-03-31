namespace Lpc.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Lpc.Application.Features.Orders;
using Lpc.Application.Features.Products;
using Lpc.Application.Features.Portfolios;
using Lpc.Application.Features.User;
using Lpc.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Mime;
using Lpc.Infrastructure.Configs;
using Microsoft.Extensions.Options;
using Lpc.Application.Abstractions;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string httpClientName)
    {
        services.AddScoped<IDialogOrchestrator, DialogOrchestrator>();
        services.AddScoped(typeof(IDialogMiddleware<,>), typeof(DialogLoggingMiddleware<,>));
        
        services.AddHttpClient<IOrderApi, OrderApi>(httpClientName);
        services.AddHttpClient<IPortfolioApi, PortfolioApi>(httpClientName);
        services.AddHttpClient<IProductApi, ProductApi>(httpClientName);
        services.AddHttpClient<IUserProfileApi, UserProfileApi>(httpClientName);

        return services;
    }

    public static IHttpClientBuilder AddApiHttpClient(
        this IServiceCollection services,
        string httpClientName,
        string fallbackBaseAddress)
    {
        return services
            .AddHttpClient(httpClientName, (sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<BaseAddressSettings>>().Value;
                var uri = settings.ResolveBaseAddress(fallbackBaseAddress);

                client.BaseAddress = uri;
                client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            });
    }
}

public static class ApiHttpClientExtensions
{
    public static Uri ResolveBaseAddress(
        this BaseAddressSettings settings,
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
