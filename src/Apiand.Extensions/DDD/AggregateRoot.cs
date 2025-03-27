namespace Apiand.Extensions.DDD;

/// <summary>
/// Base class for aggregate roots in Domain-Driven Design.
/// </summary>
/// <remarks>
/// An aggregate root is the primary entity within an aggregate and is responsible for
/// maintaining the aggregate's invariants. It serves as the only entry point for
/// modifications to entities within the aggregate and can raise domain events to
/// communicate changes to other parts of the system.
/// </remarks>
public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the collection of domain events raised by this aggregate root.
    /// </summary>
    /// <value>
    /// A read-only collection of <see cref="DomainEvent"/> objects that have been raised but not yet dispatched.
    /// </value>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the collection of events to be dispatched.
    /// </summary>
    /// <param name="domainEvent">The <see cref="DomainEvent"/> to add.</param>
    /// <remarks>
    /// Domain events represent significant changes or actions within the aggregate that might
    /// be of interest to other parts of the system. Events are typically dispatched after the
    /// aggregate is persisted.
    /// </remarks>
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from this aggregate root.
    /// </summary>
    /// <remarks>
    /// This method is typically called after all events have been dispatched.
    /// </remarks>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}