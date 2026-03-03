namespace Lpc.Infrastructure.Behaviors;

using Lpc.Application.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Logging;

public sealed class CommandValidationBehavior<TCommand>(
    ILogger<CommandValidationBehavior<TCommand>> logger,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandPipelineBehavior<TCommand> where TCommand : ICommand
{
    public async Task HandleAsync(TCommand command, CommandHandlerDelegate next, CancellationToken ct = default)
    {
        if (!validators.Any())
        {
            await next();
            return;
        }

        logger.LogInformation("Validating {CommandType}", typeof(TCommand).Name);

        var context = new ValidationContext<TCommand>(command);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
        {
            logger.LogWarning(
                "Validation failed for {CommandType} with {ErrorCount} error(s)",
                typeof(TCommand).Name, failures.Count);

            throw new ValidationException(failures);
        }

        await next();
    }
}
