namespace Lpc.Infrastructure;

using Lpc.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        return services;
    }
}
