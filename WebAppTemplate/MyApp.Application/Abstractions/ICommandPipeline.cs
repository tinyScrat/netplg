namespace MyApp.Application.Abstractions;

using System.Collections.Concurrent;
using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

/// <summary>
/// A service for executing commands through a pipeline of effects, with support for cancellation and disposal.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandPipeline<in TCommand> where TCommand : ICommand
{
    IDisposable Execute(TCommand command);
}

/// <summary>
/// A command that produces a result of type TResult when executed.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ICommandResultHandler<TResult>
{
    Task HandleAsync(TResult result, CancellationToken ct);
}

/// <summary>
/// A command pipeline that wraps another pipeline and ensures that only one instance of
/// a command with the same key is executed at a time.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <param name="inner"></param>
/// <param name="keyProvider"></param>
internal sealed class IdempotentCommandPipeline<TCommand, TResult>(
    IServiceScopeFactory scopeFactory,
    ICommandResultHandler<TResult> resultHandler,
    ICommandKey<TCommand> keyProvider,
    ILogger<IdempotentCommandPipeline<TCommand, TResult>> logger) : ICommandPipeline<TCommand>
    where TCommand : ICommand<TResult>
{
    private sealed record InFlight(
        CancellationTokenSource Cts,
        IDisposable Subscription,
        IServiceScope Scope);

    private readonly ConcurrentDictionary<string, InFlight> _inFlight = new();

    public IDisposable Execute(TCommand command)
    {
        var key = keyProvider.GetKey(command);

        var scope = scopeFactory.CreateScope();
        var cts = new CancellationTokenSource();

        IEffect<TCommand, TResult> effect;

        try
        {
            effect = scope.ServiceProvider.GetRequiredService<IEffect<TCommand, TResult>>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to resolve IEffect for command key {Key}.", key);
            cts.Dispose();
            scope.Dispose();
            return Disposable.Empty;
        }

        // Create temporary placeholder
        var entry = new InFlight(cts, Disposable.Empty, scope);

        if (!_inFlight.TryAdd(key, entry))
        {
            logger.LogWarning("Command with key {Key} already in-flight.", key);
            IdempotentCommandPipeline<TCommand, TResult>.DisposeEntry(entry);
            return Disposable.Empty;
        }

        var subscription =
            effect
                .Handle(command, cts.Token)
                .SelectMany(result =>
                    Observable.FromAsync(ct =>
                        resultHandler.HandleAsync(result, ct)))
                .Subscribe(
                    _ => { },
                    ex =>
                    {
                        logger.LogError(ex, "Error executing command with key {Key}", key);
                        Cleanup(key);
                    },
                    () =>
                    {
                        logger.LogInformation("Command {Key} completed.", key);
                        Cleanup(key);
                    });

        // Update entry with real subscription
        _inFlight[key] = entry with { Subscription = subscription };

        return Disposable.Create(() => Cleanup(key));
    }

    private void Cleanup(string key)
    {
        if (_inFlight.TryRemove(key, out var entry))
        {
            IdempotentCommandPipeline<TCommand, TResult>.DisposeEntry(entry);
        }
    }

    private static void DisposeEntry(InFlight entry)
    {
        entry.Cts.Cancel();
        entry.Subscription.Dispose();
        entry.Cts.Dispose();
        entry.Scope.Dispose();
    }
}

public static class CqrsRegistrationExtensions
{
    public static IServiceCollection AddIdempotentCommand<
        TCommand,
        TKeyProvider,
        TEffect,
        TResult,
        TResultHandler>(
        this IServiceCollection services)
        where TEffect : class, IEffect<TCommand, TResult>
        where TKeyProvider : class, ICommandKey<TCommand>
        where TResultHandler : class, ICommandResultHandler<TResult>
        where TCommand : ICommand<TResult>
    {
        // Effect is scoped (uses transient typed HttpClient)
        services.AddScoped<IEffect<TCommand, TResult>, TEffect>();

        // Key provider can be singleton (stateless)
        services.AddSingleton<ICommandKey<TCommand>, TKeyProvider>();

        // Result handler can be singleton (stateless)
        services.AddSingleton<ICommandResultHandler<TResult>, TResultHandler>();

        // Pipeline is SINGLETON — it owns a scope per Execute() call
        services.AddSingleton<
            ICommandPipeline<TCommand>,
            IdempotentCommandPipeline<TCommand, TResult>>();

        return services;
    }
}
