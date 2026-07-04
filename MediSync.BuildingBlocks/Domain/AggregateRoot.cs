namespace MediSync.BuildingBlocks.Domain;

/// <summary>
/// Base class for all aggregate roots.
/// Provides a unique identifier and support for domain events.
/// </summary>
public abstract class AggregateRoot
{
    /// <summary>
    /// Gets the unique identifier of the aggregate.
    /// Can only be assigned by the aggregate itself or its derived classes.
    /// </summary>
    public Guid Id { get; protected set; }

    // Stores domain events raised by the aggregate during the current transaction.
    // Events are collected here and published after the transaction completes.
    private readonly List<IDomainEvent> _domainEvent = new();

    /// <summary>
    /// Gets the domain events raised by the aggregate.
    /// The returned collection is read-only to prevent external modification.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvent.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate.
    /// Only the aggregate itself can raise its own domain events.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvent.Add(domainEvent);

    /// <summary>
    /// Removes all domain events from the aggregate.
    /// Called after the events have been successfully published.
    /// </summary>
    public void ClearDomainEvents()
        => _domainEvent.Clear();
}