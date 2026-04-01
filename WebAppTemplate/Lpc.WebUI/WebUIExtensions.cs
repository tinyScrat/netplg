namespace Lpc.WebUI;

using Lpc.Application.Services;
using Lpc.WebUI.Services;
using Lpc.WebUI.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Lpc.Infrastructure;
using Microsoft.Extensions.Options;
using Lpc.Infrastructure.Configs;
using Lpc.Application.Abstractions;
using Microsoft.AspNetCore.Components;
using Lpc.Presentation.Features;

internal static class WebUIExtensions
{
    public static IServiceCollection AddWebUIFeatures(
        this IServiceCollection services)
    {
        services.AddSingleton<IKeyValueStorage, BrowserLocalStorage>();

        services
            .AddScoped<SessionExpiredSubscriber>()
            .AddScoped<AuthStateChangedEventPublisher>();

        services.RegisterDialogs();
        
        services.AddTransient<AuthorizeViewExViewModel>();

        return services;
    }

    public static IServiceProvider UseWebUIFeatures(this IServiceProvider sp)
    {
        return sp;
    }

    public static IServiceCollection AddApiHttpClientWithAuth(
        this IServiceCollection services,
        string httpClientName,
        IConfiguration configuration,
        string fallbackBaseAddress)
    {
        services
            .Configure<BaseAddressSettings>(configuration.GetSection(BaseAddressSettings.SectionName))
            .AddTransient<AuthDelegatingHandler>()
            .AddApiHttpClient(httpClientName, fallbackBaseAddress)
            // OUTERMOST: can catch exceptions from inner handlers (incl. AuthorizationMessageHandler)
            .AddHttpMessageHandler<AuthDelegatingHandler>()
            // INNER: attaches access token for configured URLs
            .AddHttpMessageHandler(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("ApiHttpClient");

                var settings = sp.GetRequiredService<IOptions<BaseAddressSettings>>().Value;
                var uri = settings.ResolveBaseAddress(fallbackBaseAddress);

                logger.LogInformation("API BaseAddress: {BaseAddress}", uri);

                return sp.GetRequiredService<AuthorizationMessageHandler>()
                    .ConfigureHandler([uri.ToString()]);
            });

        return services;
    }

    private static IServiceCollection RegisterDialogs(this IServiceCollection services)
    {
        services.AddScoped<INavigationDialogCancellation, BlazorNavigationDialogCancellation>();
        services.AddDialog<ConfirmDialog, ConfirmDialogRequest, bool>();

        return services;
    }

    public static IServiceCollection AddDialog<TComponent, TRequest, TResult>(
        this IServiceCollection services)
        where TComponent : ComponentBase, IDialogComponent<TRequest, TResult>
        where TRequest : IDialogRequest<TResult>
    {
        return services.AddScoped<
            IDialogHandler<TRequest, TResult>,
            RadzenDialogHandler<TComponent, TRequest, TResult>>();
    }
}
