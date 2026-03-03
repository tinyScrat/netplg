namespace Lpc.Infrastructure;

using Lpc.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.DependencyInjection;

public static class InfraExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddActorSystem();

        return services;
    }

    public static void UseInfrastructure(this IServiceProvider sp)
    {
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
        Log.SetLoggerFactory(loggerFactory);
    }

    private static IServiceCollection AddActorSystem(this IServiceCollection services)
    {
        services
            .AddSingleton(sp =>
            {
                var actorSystemConfig =
                    ActorSystemConfig.Setup()
                    .WithConfigureProps(props => props.WithStartDeadline(TimeSpan.FromSeconds(1)))
                    .WithActorRequestTimeout(TimeSpan.FromSeconds(10));
                var actorSystem =
                    new ActorSystem(actorSystemConfig)
                        .WithServiceProvider(sp);
                return actorSystem;
            });

        return services;
    }
}
