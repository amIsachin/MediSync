using MediSync.Auth.Domain.Utilities.Enums;
using Microsoft.AspNetCore.Identity;

namespace MediSync.Auth.Infrastrucure.Identity;

// ApplicationUser extends IdentityUser — ASP.NET Identity's user class
// IdentityUser gives us: UserName, Email, PasswordHash, PhoneNumber
//                        EmailConfirmed, LockoutEnabled, AccessFailedCount, etc.
// We get all password hashing, lockout, and token generation for FREE

// This class lives in INFRASTRUCTURE — never in Domain
// If Microsoft changes IdentityUser tomorrow, only this file changes
public class ApplicationUser : IdentityUser
{
    // DomainUserId links this Identity record to your clean Domain User
    // Two records exist for each user:
    //   AspNetUsers table → ApplicationUser (Identity handles auth mechanics)
    //   Users table       → User (your domain aggregate, business rules)
    public Guid DomainUserId { get; set; }

    // We duplicate these here so Identity can use them
    // (IdentityUser only has UserName and Email by default)
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

}
