using MediSync.BuildingBlocks.Domain;

namespace MediSync.Auth.Domain.Events;

/// <summary>
/// Raised when a new user successfully registers.
/// This event is published so other services (e.g. Notification API)
/// can perform post-registration actions such as sending a verification email.
/// </summary>
public sealed record UserRegisteredEvent(Guid EventId, DateTime OccurredAt, Guid UserId, string Email, string Role) : IDomainEvent
{
    /// <summary>
    /// Creates a new <see cref="UserRegisteredEvent"/> with
    /// an automatically generated event ID and UTC timestamp.
    /// </summary>
    public UserRegisteredEvent(Guid userId, string email, string role)
        : this(Guid.NewGuid(), DateTime.UtcNow, userId, email, role)
    { }

    public Guid Id => EventId;
}

/// <summary>
/// Raised when a user's account is successfully activated.
/// Other services can react by sending welcome emails,
/// enabling additional features, or auditing the activation.
/// </summary>
public sealed record UserActivatedEvent(Guid EventId, DateTime OccurredAt, Guid UserId, string Email) : IDomainEvent
{
    /// <summary>
    /// Creates a new <see cref="UserActivatedEvent"/> with
    /// an automatically generated event ID and UTC timestamp.
    /// </summary>
    public UserActivatedEvent(Guid userId, string email)
        : this(Guid.NewGuid(), DateTime.UtcNow, userId, email)
    { }

    public Guid Id => EventId;
}

/// <summary>
/// Raised when a user's account is deactivated.
/// Consumers may revoke active sessions, stop notifications,
/// or archive related resources.
/// </summary>
public sealed record UserDeactivatedEvent(Guid EventId, DateTime OccurredAt, Guid UserId) : IDomainEvent
{
    /// <summary>
    /// Creates a new <see cref="UserDeactivatedEvent"/> with
    /// an automatically generated event ID and UTC timestamp.
    /// </summary>
    public UserDeactivatedEvent(Guid userId)
        : this(Guid.NewGuid(), DateTime.UtcNow, userId)
    { }

    public Guid Id => EventId;
}