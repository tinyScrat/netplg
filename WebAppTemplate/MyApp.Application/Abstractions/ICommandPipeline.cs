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
}

/// <summary>
/// A command pipeline that executes a single effect and invokes a callback with the result.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <param name="effect"></param>
/// <param name="onResult"></param>
internal sealed class CommandPipeline<TCommand, TResult>(
    IEffect<TCommand, TResult> effect,
    Action<TResult> onResult)
    : ICommandPipeline<TCommand>
    where TCommand : ICommand<TResult>
{
    public IDisposable Execute(TCommand command) =>
        effect
            .Handle(command, CancellationToken.None)
            .Subscribe(onResult);
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
    CommandPipeline<TCommand, TResult> inner,
    ICommandKey<TCommand> keyProvider)
    : ICommandPipeline<TCommand>
    where TCommand : ICommand<TResult>
{
    private readonly ConcurrentDictionary<string, IDisposable> _inFlight = new();

    public IDisposable Execute(TCommand command)
    {
        var key = keyProvider.GetKey(command);

        // Already executing → no-op
        if (_inFlight.ContainsKey(key))
            return Disposable.Empty;

        var subscription =
            inner.Execute(command);

        if (!_inFlight.TryAdd(key, subscription))
        {
            subscription.Dispose();
            return Disposable.Empty;
        }

        return Disposable.Create(() =>
        {
            subscription.Dispose();
            _inFlight.TryRemove(key, out _);
        });
    }
}
