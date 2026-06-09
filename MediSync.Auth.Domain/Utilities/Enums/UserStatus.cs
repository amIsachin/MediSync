namespace MediSync.Auth.Domain.Utilities.Enums;

public enum UserStatus
{
    // User registered but has not verified their email yet
    // Cannot login until they verify
    PendingVerification = 1,

    // Normal working state — user can login and use the system
    Active = 2,

    // Temporarily blocked — cannot login
    // Admin can unsuspend
    Suspended = 3,

    // Permanently disabled — user requested account deletion
    // or admin permanently banned them
    // Cannot be reversed (only SuperAdmin can)
    Deactivated = 4
}
