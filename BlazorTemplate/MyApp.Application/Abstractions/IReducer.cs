namespace MyApp.Application.Abstractions;

using System.Reactive.Linq;

public interface IReducer<TState, in TCommand, in TResult>
{
    TState Reduce(TState state, TCommand command, TResult result);
}

public sealed class CommandDispatcher<TState>(AsyncState<TState> state)
{
    public IObservable<TResult> Dispatch<TCommand, TResult>(
        TCommand command,
        IEffect<TCommand, TResult> effect,
        IReducer<TState, TCommand, TResult> reducer)
        where TCommand : ICommand<TResult>
    {
        return state.TrackAsync(
            effect.Handle(command)
        )
        .Do(result =>
        {
            state.Data.Update(state =>
                reducer.Reduce(state, command, result));
        });
    }
}
