using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.Auth.Infrastrucure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MediSync.Auth.Infrastrucure.Services;

/// <summary>
/// Provides operations for creating and managing ASP.NET Identity users.
/// Responsible for synchronizing domain users with the Identity store.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Creates a new ASP.NET Identity user associated with the specified domain user.
    /// </summary>
    /// <param name="domainUserId">
    /// The identifier of the corresponding domain user.
    /// </param>
    /// <param name="firstName">
    /// The user's first name.
    /// </param>
    /// <param name="lastName">
    /// The user's last name.
    /// </param>
    /// <param name="email">
    /// The user's email address.
    /// </param>
    /// <param name="password">
    /// The user's plaintext password.
    /// </param>
    /// <param name="role">
    /// The user's role.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// An <see cref="IdentityCreationResult"/> indicating whether the
    /// Identity user was created successfully.
    /// </returns>
    public async Task<IdentityCreationResult> CreateUserAsync(Guid domainUserId, string firstName, string lastName, string email, string password, string role, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = Enum.Parse<UserRole>(role),
            Status = UserStatus.PendingVerification,
            CreatedAt = DateTime.UtcNow,
            DomainUserId = domainUserId
        };

        IdentityResult identityResult = await _userManager.CreateAsync(user, password);

        if (!identityResult.Succeeded)
        {
            var error = identityResult.Errors.First();
            return new IdentityCreationResult(false, error.Code, error.Description);
        }

        return new IdentityCreationResult(true, null, null);
    }
}