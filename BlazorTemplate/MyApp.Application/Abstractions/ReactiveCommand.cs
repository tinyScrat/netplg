namespace MyApp.Application.Abstractions;

using System.Reactive.Linq;
using System.Reactive.Subjects;

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
        IAsyncTracker? asyncState = null,
        Action<TCommand, TResult>? onResult = null)
    {
        var commandResults =
            _commands
                .SelectMany(cmd =>
                    Observable.Defer(() =>
                    {
                        var source = effect.Handle(cmd);

                        if (asyncState != null)
                            source = asyncState.TrackAsync(source);

                        // Prevent pipeline from terminating on error
                        return source
                            .Select(result => (Command: cmd, Result: result))
                            .Catch(Observable.Empty<(TCommand Command, TResult Result)>());
                    }))
                .Publish()
                .RefCount();

        _results = commandResults.Select(x => x.Result);

        // Keep pipeline alive
        _subscription = commandResults.Subscribe(
            onNext: x => onResult?.Invoke(x.Command, x.Result), // Fire-and-forget callback
            onError: _ => { } // upstream errors are already caught; this should rarely run
        );
    }

    /// <summary>
    /// Fire the command
    /// </summary>
    public void Execute(TCommand command) => _commands.OnNext(command);

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
    /// <summary>
    /// Create a ReactiveCommand from an execute lambda (no custom IEffect class needed).
    /// </summary>
    public static ReactiveCommand<TCommand, TResult> CreateFromObservable<TCommand, TResult>(
        Func<TCommand, IObservable<TResult>> execute,
        IAsyncTracker? asyncState = null,
        Action<TCommand, TResult>? onResult = null)
        where TCommand : ICommand<TResult>
    {
        var effect = new LambdaEffect<TCommand, TResult>(execute);

        return new ReactiveCommand<TCommand, TResult>(
            effect: effect,
            asyncState: asyncState,
            onResult: onResult
        );
    }

    /// <summary>
    /// Create a ReactiveCommand that also reduces AsyncState.Data when results arrive.
    /// This avoids the "must subscribe or nothing happens" pitfall of CommandDispatcher.
    /// </summary>
    public static ReactiveCommand<TCommand, TResult> CreateWithReducer<TState, TCommand, TResult>(
        AsyncState<TState> state,
        IEffect<TCommand, TResult> effect,
        IReducer<TState, TCommand, TResult> reducer)
        where TCommand : ICommand<TResult>
    {
        return new ReactiveCommand<TCommand, TResult>(
            effect: effect,
            asyncState: state,
            onResult: (command, result) =>
                // Reduce state once per successful result emission
                state.Data.Update(current => reducer.Reduce(current, command, result)));
    }
}
