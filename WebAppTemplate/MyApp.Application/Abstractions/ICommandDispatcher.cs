namespace MyApp.Application.Abstractions;

using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
internal sealed class CommandDispatcher(
    IServiceProvider sp,
    ILogger<CommandDispatcher> logger) : ICommandDispatcher, IDisposable
{
    private readonly CompositeDisposable _disposables = [];

    public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
    {
        // GetServices<> returns ALL registered pipelines for this command type
        var pipelines = sp.GetServices<ICommandPipeline<TCommand>>();

        var count = 0;
        foreach (var pipeline in pipelines)
        {
            count++;
            logger.LogInformation("Dispatching {Command} to pipeline #{Index}", typeof(TCommand).Name, count);
            pipeline.Execute(command);
        }

        if (count == 0)
        {
            logger.LogWarning("No pipeline registered for {Command}", typeof(TCommand).Name);
        }
    }

    public void Dispose() => _disposables.Dispose();
}
