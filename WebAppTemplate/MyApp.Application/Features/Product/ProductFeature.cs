namespace MyApp.Application.Features.Products;

using Microsoft.Extensions.DependencyInjection;

public static class ProductFeature
{
    public static IServiceCollection AddProductFeature(this IServiceCollection services)
    {
        services.AddSingleton<SaveProductEffect>();
        services.AddSingleton<PublishProductEffect>();
        services.AddSingleton<LoadProductEffect>();

        services.AddTransient<SaveProductReducer>();

        return services;
    }
}
