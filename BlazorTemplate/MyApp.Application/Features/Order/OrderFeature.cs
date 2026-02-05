namespace MyApp.Application.Features.Orders;

using Microsoft.Extensions.DependencyInjection;

public static class OrderFeature
{
    public static IServiceCollection AddOrderFeature(this IServiceCollection services)
    {
        services.AddSingleton<LoadOrderEffect>();
        services.AddSingleton<SaveOrderDraftEffect>();
        
        services.AddTransient<EditOrderViewModel>();

        return services;
    }
}
