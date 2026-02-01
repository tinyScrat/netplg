namespace MyApp.Application.Features.Products;

using Microsoft.Extensions.DependencyInjection;

public static class ProductFeature
{
    public static IServiceCollection AddProductFeature(this IServiceCollection service)
    {
        service.AddSingleton<SaveProductEffect>();
        service.AddTransient<SaveProductReducer>();

        service.AddSingleton<PublishProductEffect>();
        service.AddSingleton<LoadProductEffect>();

        service.AddTransient<ProductViewModel>();
        return service;
    }
}
