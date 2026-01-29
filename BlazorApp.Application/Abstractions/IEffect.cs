namespace BlazorApp.Application.Abstractions;

using System.Reactive;

public interface ICommand { }

public interface IEffect<in TCommand> where TCommand : ICommand
{
    IObservable<Unit> Handle(TCommand command);
}
