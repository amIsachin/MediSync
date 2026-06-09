namespace MediSync.BuildingBlocks.Domain;

/// <summary>
/// IDomainEvent is a marker interface that represents a domain event in the system.
/// It says: this object is a domain event announcement.
/// Every domain event in every service implements this interface
/// </summary>
public interface IDomainEvent
{
    // Every event has a unique ID so we can track it
    // and prevent processing the same event twice
    Guid Id { get; }

    // When exactly did this event happen?
    // Always UTC — never local time in distributed systems
    DateTime OccurredAt { get; }
}