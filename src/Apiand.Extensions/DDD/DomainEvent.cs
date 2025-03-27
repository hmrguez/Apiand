namespace Apiand.Extensions.DDD;

/// <summary>
/// Base record representing a domain event in Domain-Driven Design.
/// </summary>
/// <remarks>
/// Domain events represent significant state changes or actions within an aggregate.
/// They are used to communicate these changes to other parts of the system in a 
/// loosely coupled manner, enabling reactive behavior and maintaining the Single
/// Responsibility Principle.
/// </remarks>
public abstract record DomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this domain event.
    /// </summary>
    /// <value>
    /// A new <see cref="Guid"/> automatically generated when the event is created.
    /// </value>
    public Guid Id { get; } = Guid.NewGuid();
    
    /// <summary>
    /// Gets the timestamp when this domain event occurred.
    /// </summary>
    /// <value>
    /// The UTC date and time when the event was instantiated.
    /// </value>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}