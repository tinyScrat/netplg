namespace MyApp.Application.Abstractions;

/// <summary>
/// Represents an effect that can be executed in response to a command.
/// Effects are used to perform side effects, such as updating the state
/// of the application or sending notifications.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IEffect<in TCommand, out TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the specified command and returns an observable of the result.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    IObservable<TResult> Handle(TCommand command, CancellationToken ct = default);
}

/// <summary>
/// Represents a lambda effect that can be executed in response to a command.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <param name="execute"></param>
internal sealed class LambdaEffect<TCommand, TResult>(Func<TCommand, IObservable<TResult>> execute)
    : IEffect<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public IObservable<TResult> Handle(TCommand command, CancellationToken ct = default) => execute(command);
}

/// <summary>
/// Represents an adapter that allows an IEffect to be used as an IEffect with a different command type.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <param name="inner"></param>
internal sealed class EffectAdapter<TCommand, TResult>(IEffect<TCommand, TResult> inner)
    : IEffect<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public IObservable<TResult> Handle(TCommand command, CancellationToken ct) => inner.Handle(command, ct);
}
