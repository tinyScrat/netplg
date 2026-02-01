namespace MyApp.Application.Abstractions;

public interface ICommand<TResult> { }

public interface IEffect<in TCommand, out TResult> where TCommand : ICommand<TResult>
{
    IObservable<TResult> Handle(TCommand command);
}

internal sealed class LambdaEffect<TCommand, TResult>
    : IEffect<TCommand, TResult> where TCommand : ICommand<TResult>
{
    private readonly Func<TCommand, IObservable<TResult>> _execute;

    public LambdaEffect(Func<TCommand, IObservable<TResult>> execute)
    {
        _execute = execute;
    }

    public IObservable<TResult> Handle(TCommand command)
        => _execute(command);
}
