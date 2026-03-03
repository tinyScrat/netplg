namespace Lpc.Application;

using Microsoft.Extensions.DependencyInjection;

public static class AppExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services, e.g.:
        // services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        // services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        // services.AddScoped<IEventBus, EventBus>();

        return services;
    }
}
