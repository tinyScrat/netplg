namespace MyApp.Application.Abstractions;

using System.Collections.Concurrent;
using System.Reactive.Disposables;

/// <summary>
/// A service for executing commands through a pipeline of effects, with support for cancellation and disposal.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandPipeline<in TCommand> where TCommand : ICommand
{
    IDisposable Execute(TCommand command);
    void Cancel(string key);
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
    IEffect<TCommand, TResult> effect,
    Action<TResult> onResult,
    ICommandKey<TCommand> keyProvider)
    : ICommandPipeline<TCommand>
    where TCommand : ICommand<TResult>
{
    private sealed record InFlight(
        CancellationTokenSource Cts,
        IDisposable Subscription);

    private readonly ConcurrentDictionary<string, InFlight> _inFlight = new();

    public IDisposable Execute(TCommand command)
    {
        var key = keyProvider.GetKey(command);

        // Already running → no-op
        if (_inFlight.ContainsKey(key))
            return Disposable.Empty;

        var cts = new CancellationTokenSource();

        var subscription =
            effect
                .Handle(command, cts.Token)
                .Subscribe(
                    onResult,
                    _ => Cleanup(key),
                    () => Cleanup(key));

        var entry = new InFlight(cts, subscription);

        if (!_inFlight.TryAdd(key, entry))
        {
            subscription.Dispose();
            cts.Cancel();
            cts.Dispose();
            return Disposable.Empty;
        }

        return Disposable.Create(() => Cancel(key));
    }

    public void Cancel(string key)
    {
        if (_inFlight.TryRemove(key, out var entry))
        {
            entry.Cts.Cancel();
            entry.Subscription.Dispose();
            entry.Cts.Dispose();
        }
    }

    private void Cleanup(string key)
    {
        if (_inFlight.TryRemove(key, out var entry))
        {
            entry.Subscription.Dispose();
            entry.Cts.Dispose();
        }
    }
}
