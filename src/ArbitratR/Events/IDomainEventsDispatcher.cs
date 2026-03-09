namespace ArbitratR.Events
{
    /// <summary>
    /// Defines a contract for dispatching domain events.
    /// </summary>
    public interface IDomainEventsDispatcher
    {
        /// <summary>
        /// Dispatches the specified domain events asynchronously.
        /// </summary>
        /// <param name="domainEvents">The collection of domain events to dispatch.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous dispatch operation.</returns>
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
