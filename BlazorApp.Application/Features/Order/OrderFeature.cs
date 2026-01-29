namespace BlazorApp.Application.Features.Orders;

using Microsoft.Extensions.DependencyInjection;

public static class OrderFeature
{
    public static IServiceCollection AddOrderFeature(this IServiceCollection services)
    {
        services.AddTransient<EditOrderViewModel>();

        return services;
    }
}
