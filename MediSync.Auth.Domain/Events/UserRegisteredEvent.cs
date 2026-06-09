using MediSync.BuildingBlocks.Domain;

namespace MediSync.Auth.Domain.Events;

public sealed record UserRegisteredEvent(Guid EventId, DateTime OccurredAt, Guid UserId, string Email, string Role) : IDomainEvent
{
    public UserRegisteredEvent(Guid userId, string email, string role)
        : this(Guid.NewGuid(), DateTime.UtcNow, userId, email, role)
    { }

    public Guid Id => throw new NotImplementedException();
}

public sealed record UserActivatedEvent(Guid EventId, DateTime OccurredAt, Guid UserId, string Email) : IDomainEvent
{
    public UserActivatedEvent(Guid userId, string email)
        : this(Guid.NewGuid(), DateTime.UtcNow, userId, email)
    { }

    public Guid Id => throw new NotImplementedException();
}

public sealed record UserDeactivatedEvent(Guid EventId, DateTime OccurredAt, Guid UserId) : IDomainEvent
{
    public UserDeactivatedEvent(Guid userId)
        : this(Guid.NewGuid(), DateTime.UtcNow, userId)
    { }

    public Guid Id => throw new NotImplementedException();
}