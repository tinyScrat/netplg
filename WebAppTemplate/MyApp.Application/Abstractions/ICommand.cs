namespace MyApp.Application.Abstractions;

/// <summary>
/// A command that can be dispatched to an effect, optionally producing a result.
/// </summary>
public interface ICommand { }

/// <summary>
/// A command that can be dispatched to an effect, optionally producing a result.
/// The type parameter TResult represents the type of the result produced by the command, if any.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ICommand<TResult> : ICommand { }

/// <summary>
/// A service for extracting a key from a command, used for caching and deduplication purposes.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandKey<in TCommand>
{
    /// <summary>
    /// Extracts a key from the given command. Commands with the same key
    /// are considered identical for caching and deduplication purposes.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    string GetKey(TCommand command);

    /// <summary>
    /// Returns the key of the currently executing command, if any. This can be used to implement
    /// cancellation of in-flight commands when the relevant context changes (e.g. user logs out).
    /// </summary>
    /// <returns></returns>
    string? GetCurrentKey();
}
