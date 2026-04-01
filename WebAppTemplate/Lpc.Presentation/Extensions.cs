namespace Lpc.Presentation;

using Lpc.Presentation.Features;
using Microsoft.Extensions.DependencyInjection;

public static class ViewModelExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services
            .AddTransient<EditOrderViewModel>()
            .AddTransient<HeaderViewModel>()
            .AddTransient<MainMenuViewModel>()
            .AddTransient<OrdersViewModel>()
            .AddTransient<PortfolioViewModel>()
            .AddTransient<ProductViewModel>();

        return services;
    }
}
