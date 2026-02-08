namespace MyApp.Application.Abstractions;

using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;


/// <summary>
/// A service for dispatching commands to their corresponding effects.
/// </summary>
public interface ICommandDispatcher
{
    void Dispatch<TCommand>(TCommand command) where TCommand : ICommand;
}

/// <summary>
/// A command dispatcher that resolves command pipelines from the service provider and executes them.
/// </summary>
/// <param name="sp"></param>
internal sealed class CommandDispatcher(IServiceProvider sp) : ICommandDispatcher, IDisposable
{
    private readonly CompositeDisposable _disposables = [];

    public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
    {
        // Resolve all pipelines for this command
        var pipelines =
            sp.GetServices<ICommandPipeline<TCommand>>();

        foreach (var pipeline in pipelines)
        {
            var sub = pipeline.Execute(command);
            _disposables.Add(sub);
        }
    }

    public void Dispose() => _disposables.Dispose();
}
