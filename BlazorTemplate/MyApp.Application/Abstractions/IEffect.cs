namespace MyApp.Application.Abstractions;

public interface ICommand<TResult> { }

public interface IEffect<in TCommand, out TResult> where TCommand : ICommand<TResult>
{
    IObservable<TResult> Handle(TCommand command, CancellationToken ct = default);
}

internal sealed class LambdaEffect<TCommand, TResult>(Func<TCommand, IObservable<TResult>> execute)
    : IEffect<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public IObservable<TResult> Handle(TCommand command, CancellationToken ct = default) => execute(command);
}
