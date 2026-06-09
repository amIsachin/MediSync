namespace MediSync.BuildingBlocks.Domain;

// abstract = you cannot create AggregateRoot directly
// Only User, Prescription, etc. can be created — and they extend this
public abstract class AggregateRoot
{
    // Every aggregate has an Id
    // protected set = only this class and subclasses (User, Prescription) can set it
    // external code can READ it but never SET it
    public Guid Id { get; protected set; }

    // Private list — nobody outside this class can add/remove events directly
    // The aggregate is in full control of its own events
    private readonly List<IDomainEvent> _domainEvent = new();

    // Public read-only view of the events
    // External code can READ the list but never modify it
    // AsReadOnly() creates a wrapper that throws if someone tries to add/remove
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvent.AsReadOnly();

    // protected = only subclasses (User, Prescription) can call this
    // external code cannot raise events — only the aggregate itself can
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvent.Add(domainEvent);

    // Called AFTER events are published to the message bus
    // Clears the list so they are not published again
    public void ClearDomainEvents()
        => _domainEvent.Clear();
}