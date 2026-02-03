namespace MyApp.Application.Abstractions;

public interface ICommand<TResult> { }

public interface IEffect<in TCommand, out TResult> where TCommand : ICommand<TResult>
{
    IObservable<TResult> Handle(TCommand command);
}

internal sealed class LambdaEffect<TCommand, TResult>(Func<TCommand, IObservable<TResult>> execute)
    : IEffect<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public IObservable<TResult> Handle(TCommand command) => execute(command);
}
