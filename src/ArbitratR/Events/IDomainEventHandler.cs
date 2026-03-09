namespace ArbitratR.Events
{
    /// <summary>
    /// Defines a handler for a specific domain event type.
    /// </summary>
    /// <typeparam name="T">The type of domain event to handle.</typeparam>
    public interface IDomainEventHandler<in T> where T : IDomainEvent
    {
        /// <summary>
        /// Handles the specified domain event.
        /// </summary>
        /// <param name="domainEvent">The domain event to handle.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Handle(T domainEvent, CancellationToken cancellationToken);
    }
}
