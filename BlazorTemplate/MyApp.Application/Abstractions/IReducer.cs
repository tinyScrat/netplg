namespace MyApp.Application.Abstractions;

using System.Reactive.Linq;

public interface IReducer<TState, in TCommand, in TResult>
{
    TState Reduce(TState state, TCommand command, TResult result);
}

/// <summary>
/// Executes a command effect and applies its result to the given AsyncState via a reducer.
/// Note: work starts only when the returned observable is subscribed.
/// </summary>
public sealed class CommandDispatcher<TState>(AsyncState<TState> state)
{
    public IObservable<TResult> Dispatch<TCommand, TResult>(
        TCommand command,
        IEffect<TCommand, TResult> effect,
        IReducer<TState, TCommand, TResult> reducer)
        where TCommand : ICommand<TResult>
    {
        // Track status + apply reducer once per effect execution,
        // then share the single execution among all subscribers.
        return state
            .TrackAsync(effect.Handle(command))
            .Do(result =>
                state.Data.Update(current =>
                    reducer.Reduce(current, command, result)))
            .Publish()
            .RefCount();
    }
}
