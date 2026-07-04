using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Infrastrucure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MediSync.Auth.Infrastrucure.Services;

/// <summary>
/// Provides authentication operations using ASP.NET Core Identity.
/// Responsible for validating user credentials and returning
/// authentication-related information to the application layer.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    /// <summary>
    /// Validates the supplied email and password against ASP.NET Identity.
    /// Returns whether authentication succeeded, whether the account is locked,
    /// and the associated domain user identifier.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's plaintext password.</param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="SignInCheckResult"/> containing the authentication result,
    /// lockout status, and domain user identifier.
    /// </returns>
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