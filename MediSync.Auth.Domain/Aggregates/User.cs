using MediSync.Auth.Domain.Events;
using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.BuildingBlocks.Domain;

namespace MediSync.Auth.Domain.Aggregates;

// sealed = nobody can inherit from User
// This prevents accidental subclassing that could break domain rules
// AggregateRoot gives us: Id, RaiseDomainEvent(), DomainEvents, ClearDomainEvents()
public sealed class User : AggregateRoot
{
    // ── Properties ────────────────────────────────────────────
    // private set = only THIS class can change these values
    // External code (handlers, controllers) can READ but never WRITE directly
    // This is the core of encapsulation in DDD
    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public string Email { get; private set; } = default!;

    // Role stored as enum internally — converted to string when saving to DB
    // This gives us type safety in code AND readable data in database
    public UserRole Role { get; private set; } = default!;

    public UserStatus Status { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; } = default!;

    public DateTime? UpdatedAt { get; private set; } = default!;

    // ── Private Constructor ───────────────────────────────────
    // private = nobody can do: new User()
    // Forces everyone to use factory methods (CreatePatient, CreateDoctor)
    // Factory methods ensure the User is always in a valid state from birth
    // EF Core needs a parameterless constructor — private satisfies this requirement
    private User() { }

    // ── Factory Methods ───────────────────────────────────────
    // Each role type has its own factory method
    // This makes the code read like plain English:
    // "User.CreatePatient(firstName, lastName, email)"
    public static User CreatePatient(string firstName, string lastName, string email)
    {
        return Create(firstName, lastName, email, UserRole.Patient);
    }

    public static User CreateDoctor(string firstName, string lastName, string email)
    {
        return Create(firstName, lastName, email, UserRole.Doctor);
    }

    public static User CreateAdmin(string firstName, string lastName, string email)
    {
        return Create(firstName, lastName, email, UserRole.Admin);
    }

    // ── Behaviours ────────────────────────────────────────────
    // These are the ONLY ways to change a User's state
    // Each method validates the transition before allowing it

    // Called when patient clicks the email verification link
    public void Activate()
    {
        // Guard clause — cannot activate an already active user
        // Domain RULES live here — not in the handler, not in the controller
        if (Status != UserStatus.PendingVerification)
        {
            throw new InvalidOperationException($"Cannot activate a user with status '{Status}'. " + "Only PendingVerification users can be activated.");
        }

        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        // Announce: "This user is now active"
        // Notification.API will send a "Your account is ready" email
        RaiseDomainEvent(new UserActivatedEvent(Id, Email));
    }

    // Called by Admin when a user violates platform rules
    public void Suspended()
    {
        if (Status == UserStatus.Deactivated)
        {
            throw new InvalidOperationException("Cannot suspend a deactivated user.");
        }

        if (Status == UserStatus.Suspended)
        {
            return; // already suspended — silently ignore (idempotent)
        }

        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
        // No event raised — suspension is internal admin action
    }

    // Called when user requests account deletion
    public void Deactivate()
    {
        if (Status == UserStatus.Deactivated)
        {
            return; // already deactivated — silently ignore (idempotent)
        }

        Status = UserStatus.Deactivated;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new UserDeactivatedEvent(Id));
    }

    // ── Private Helper ────────────────────────────────────────
    // All factory methods call this one private method
    // Single place where a User is actually constructed
    // If you need to add a new property, add it here once
    private static User Create(string firstName, string lastName, string email, UserRole role)
    {
        ArgumentNullException.ThrowIfNull(firstName, nameof(firstName));
        ArgumentNullException.ThrowIfNull(email, nameof(email));

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = role,
            Status = UserStatus.PendingVerification,
            CreatedAt = DateTime.UtcNow
            // UpdatedAt = null — not set until first update
        };

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.Role.ToString()));

        return user;
    }
}
