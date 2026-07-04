using MediSync.BuildingBlocks.Common;

namespace MediSync.Auth.Domain.Errors;

/// <summary>
/// Centralized collection of domain errors related to users.
/// Using a single location for error definitions ensures consistency
/// across the application and avoids duplicate error messages.
/// </summary>
public static class UserErrors
{
    /// <summary>
    /// Returns an error indicating that the requested user could not be found.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    public static Error NotFound(Guid id) => Error.NotFound("User.NotFound", $"User with ID {id} was not found");

    /// <summary>
    /// Returns an error indicating that the specified email address
    /// is already associated with another account.
    /// </summary>
    /// <param name="email">The email address that already exists.</param>
    public static Error EmailAlreadyExists(string email) => Error.Conflict("User.EmailExists", $"Email '{email}' is already registered");

    /// <summary>
    /// Returned when the supplied email or password is invalid.
    /// The message is intentionally generic to prevent user enumeration attacks.
    /// </summary>
    public static readonly Error InvalidCredentials =
        Error.Failure(
            "User.InvalidCredentials",
            // IMPORTANT: do not say "wrong password" — security best practice
            // Attacker should not know if email exists or password is wrong
            "Email or password is incorrect");

    public static readonly Error AccountNotVerified = Error.Failure("User.NotVerified", "Please verify your email before logging in");

    public static readonly Error AccountSuspended = Error.Failure("User.Suspended", "Your account has been suspended. Contact support.");

    public static readonly Error Unauthorized = Error.Unauthorized("User.Unauthorized", "You are not authorized to perform this action");

    public static readonly Error AccountDeactivated = Error.Failure("User.Deactivated", "This account has been deactivated.");

    public static readonly Error LockedOut = Error.Failure("User.LockedOut", "Account locked due to multiple failed attempts. Try again in 15 minutes.");
}
