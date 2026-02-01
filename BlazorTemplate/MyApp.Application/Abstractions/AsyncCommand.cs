namespace MyApp.Application.Abstractions;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

// public class AsyncCommand<TCommand, TResult> : IDisposable where TCommand : ICommand
// {
//     private readonly Subject<TCommand> _commands = new();
//     private readonly AsyncStateStore<TResult> _state = new();

//     private readonly IDisposable _subscription;

//     public AsyncCommand(Func<TCommand, IObservable<TResult>> handler)
//     {
//         _subscription =
//             _commands
//                 .Select(cmd => handler(cmd))
//                 .Switch() // the last one wins
//                 .TrackAsync(_state)
//                 .Subscribe();
//     }

//     public IObserver<TCommand> Input => _commands;

//     public AsyncState<TResult> State => _state.Value;
//     public IObservable<AsyncState<TResult>> Changes => _state.Changes;

//     public bool IsExecuting => _state.IsLoading;

//     public void Execute(TCommand command) =>
//         _commands.OnNext(command);

//     public void Dispose()
//     {
//         _subscription.Dispose();
//         _commands.Dispose();
//         _state.Dispose();
//     }
// }

// public sealed class ReactiveCommand<TCommand, TResult> : IDisposable
//     where TCommand : ICommand<TResult>
// {
//     private readonly Subject<TCommand> _commands = new();
//     private readonly IDisposable _subscription;

//     public ReactiveCommand(
//         IEffect<TCommand, TResult> effect,
//         AsyncState<TResult> asyncState,
//         Action<TResult>? onResult = null)
//     {
//         _subscription =
//             _commands
//                 .SelectMany(cmd =>
//                     Observable.Defer(() =>
//                         effect
//                             .Handle(cmd)
//                             .Let(src => asyncState.TrackAsync(src))
//                             .Do(result => onResult?.Invoke(result))
//                             .Catch(Observable.Empty<TResult>())
//                     )
//                 )
//                 .Subscribe();
//     }

//     public void Execute(TCommand command)
//         => _commands.OnNext(command);

//     public void Dispose()
//     {
//         _subscription.Dispose();
//         _commands.Dispose();
//     }
// }

public static class RxExtensions
{
    public static TResult Let<TSource, TResult>(
        this TSource source,
        Func<TSource, TResult> selector)
        => selector(source);
}

public sealed class ReactiveCommand<TCommand, TResult> : IDisposable
    where TCommand : ICommand<TResult>
{
    private readonly Subject<TCommand> _commands = new();
    private readonly IObservable<TResult> _results;
    private readonly IDisposable _subscription;

    /// <summary>
    /// Constructs a reactive command pipeline.
    /// </summary>
    /// <param name="effect">Effect that executes the command</param>
    /// <param name="asyncState">Optional AsyncState to track Loading/Error</param>
    /// <param name="onResult">Optional callback for fire-and-forget commands</param>
    public ReactiveCommand(
        IEffect<TCommand, TResult> effect,
        AsyncState<TResult>? asyncState = null,
        Action<TResult>? onResult = null)
    {
        _results =
            _commands
                .SelectMany(cmd =>
                    Observable.Defer(() =>
                    {
                        var source = effect.Handle(cmd);

                        // Track async state if provided
                        if (asyncState != null)
                            source = asyncState.TrackAsync(source);

                        // Fire-and-forget callback
                        if (onResult != null)
                            source = source.Do(result => onResult(result));

                        // Prevent pipeline from terminating on error
                        return source.Catch(Observable.Empty<TResult>());
                    })
                )
                .Publish()
                .RefCount();

        // Keep pipeline alive
        _subscription = _results.Subscribe(_ => { }, _ => { });
    }

    /// <summary>
    /// Fire the command
    /// </summary>
    public void Execute(TCommand command)
        => _commands.OnNext(command);

    /// <summary>
    /// Observe results if you need them
    /// </summary>
    public IObservable<TResult> Results => _results;

    /// <summary>
    /// Dispose subscriptions
    /// </summary>
    public void Dispose()
    {
        _subscription.Dispose();
        _commands.Dispose();
    }
}

public static class ReactiveCommand
{
    public static ReactiveCommand<TCommand, TResult>
        CreateFromObservable<TInput, TCommand, TResult>(
            Func<TInput, TCommand> commandFactory,
            Func<TCommand, IObservable<TResult>> execute,
            AsyncState<TResult>? asyncState = null,
            Action<TResult>? onResult = null)
        where TCommand : ICommand<TResult>
    {
        var effect = new LambdaEffect<TCommand, TResult>(execute);

        return new ReactiveCommand<TCommand, TResult>(
            effect,
            asyncState,
            onResult
        );
    }

    /// Convenience overload when TInput == TCommand
    public static ReactiveCommand<TCommand, TResult>
        CreateFromObservable<TCommand, TResult>(
            Func<TCommand, IObservable<TResult>> execute,
            AsyncState<TResult>? asyncState = null,
            Action<TResult>? onResult = null)
        where TCommand : ICommand<TResult>
    {
        return CreateFromObservable<TCommand, TCommand, TResult>(
            commandFactory: c => c,
            execute: execute,
            asyncState: asyncState,
            onResult: onResult
        );
    }
}
