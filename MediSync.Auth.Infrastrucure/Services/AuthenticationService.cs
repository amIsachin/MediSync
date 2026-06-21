using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Infrastrucure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MediSync.Auth.Infrastrucure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<SignInCheckResult> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return new SignInCheckResult(false, false, Guid.Empty);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        return new SignInCheckResult(result.Succeeded, result.IsLockedOut, user.DomainUserId);
    }
}