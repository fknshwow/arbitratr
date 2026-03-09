namespace ArbitratR.Events
{
    /// <summary>
    /// Base class for all domain entities, providing domain event support.
    /// </summary>
    public abstract class DomainEntity
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        /// <summary>
        /// Gets a snapshot of the domain events raised by this entity.
        /// </summary>
        public List<IDomainEvent> DomainEvents => [.. _domainEvents];

        /// <summary>
        /// Clears all pending domain events from this entity.
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Raises a domain event to be dispatched after persistence.
        /// </summary>
        /// <param name="domainEvent">The domain event to raise.</param>
        public void Raise(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
