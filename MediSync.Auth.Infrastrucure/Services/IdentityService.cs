using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.Auth.Infrastrucure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MediSync.Auth.Infrastrucure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

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