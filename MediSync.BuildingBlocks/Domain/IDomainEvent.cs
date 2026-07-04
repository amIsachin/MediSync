namespace MediSync.BuildingBlocks.Domain;

/// <summary>
/// Marker interface that represents a domain event.
///
/// A domain event is an immutable notification that something important
/// has happened inside the domain model.
///
/// Examples:
/// - UserRegisteredEvent
/// - UserActivatedEvent
/// - PrescriptionCreatedEvent
///
/// The interface contains no behavior—it simply identifies an object
/// as a domain event so the infrastructure can discover and publish it.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// Used for event tracking, logging, and preventing duplicate processing.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The UTC date and time when the event occurred.
    /// Always use UTC to ensure consistency across distributed systems.
    /// </summary>
    DateTime OccurredAt { get; }
}