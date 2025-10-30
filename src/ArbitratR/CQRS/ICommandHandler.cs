using ArbitratR.Results;

namespace ArbitratR.CQRS
{
    /// <summary>
    /// Defines a handler for commands that do not return a result value.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand"/>.</typeparam>
    public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
    {
        /// <summary>
        /// Handles the specified command asynchronously.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="Result"/> indicating success or failure.</returns>
        Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Defines a handler for commands that return a result value.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by the command.</typeparam>
    public interface ICommandHandler<in TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Handles the specified command asynchronously.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="Result{TValue}"/> with the result value or an error.</returns>
        Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
