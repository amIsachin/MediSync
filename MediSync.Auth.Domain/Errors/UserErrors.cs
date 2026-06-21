using MediSync.BuildingBlocks.Common;

namespace MediSync.Auth.Domain.Errors;

// static class = no instances, just a collection of error definitions
// All user-related errors live here — one place, easy to find
public static class UserErrors
{
    // Method (not property) because the error message includes the specific id
    // Usage: UserErrors.NotFound(userId)
    public static Error NotFound(Guid id) =>
        Error.NotFound("User.NotFound", $"User with ID {id} was not found");

    // Method because message includes the specific email
    public static Error EmailAlreadyExists(string email) =>
        Error.Conflict("User.EmailExists", $"Email '{email}' is already registered");

    // Property (not method) because the message is always the same
    // readonly = cannot be changed after initialization
    public static readonly Error InvalidCredentials =
        Error.Failure(
            "User.InvalidCredentials",
            // IMPORTANT: do not say "wrong password" — security best practice
            // Attacker should not know if email exists or password is wrong
            "Email or password is incorrect");

    public static readonly Error AccountNotVerified =
        Error.Failure("User.NotVerified", "Please verify your email before logging in");

    public static readonly Error AccountSuspended =
        Error.Failure("User.Suspended", "Your account has been suspended. Contact support.");

    public static readonly Error Unauthorized =
        Error.Unauthorized("User.Unauthorized", "You are not authorized to perform this action");

    public static readonly Error AccountDeactivated =
        Error.Failure("User.Deactivated", "This account has been deactivated.");

    public static readonly Error LockedOut =
        Error.Failure("User.LockedOut", "Account locked due to multiple failed attempts. Try again in 15 minutes.");
}
