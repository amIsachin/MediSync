using MediatR;
using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Domain.Aggregates;
using MediSync.Auth.Domain.Errors;
using MediSync.Auth.Domain.Interfaces;
using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.BuildingBlocks.Common;

namespace MediSync.Auth.Application.Commands.LoginUser;

/// <summary>
/// Handles the user login process.
///
/// Workflow:
/// 1. Validate the user's credentials using ASP.NET Identity.
/// 2. Check whether the account is locked.
/// 3. Retrieve the corresponding domain user.
/// 4. Verify the account is allowed to sign in.
/// 5. Generate a JWT access token.
/// 6. Return the authenticated user's details.
///
/// Returns either a successful <see cref="LoginResponse"/> or
/// a domain error describing why the login request failed.
/// </summary>
public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenService _tokenService;

    public LoginUserHandler(IUserRepository userRepository, IAuthenticationService authenticationService, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Processes the login request by validating the user's credentials,
    /// checking the account status, generating a JWT token,
    /// and returning the authenticated user's details.
    ///
    /// Returns a failure result if:
    /// • The credentials are invalid.
    /// • The account is locked.
    /// • The user cannot be found.
    /// • The account is suspended.
    /// • The account has been deactivated.
    /// </summary>
    /// <param name="request">
    /// Contains the user's email address and password.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A successful <see cref="LoginResponse"/> containing the JWT token
    /// and user information, or a failure <see cref="Result{T}"/>
    /// containing the appropriate domain error.
    /// </returns>
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.CheckPasswordAsync(request.Email, request.Password, cancellationToken);

        if (result.IsLockedOut)
        {
            return Result<LoginResponse>.Failure(UserErrors.LockedOut);
        }

        if (!result.Succeeded || result?.DomainUserId == null)
        {
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);
        }

        User? userExists = await _userRepository.GetByIdAsync(result.DomainUserId, cancellationToken);

        if (userExists is null)
        {
            return Result<LoginResponse>.Failure(UserErrors.NotFound(result.DomainUserId));
        }

        if (userExists.Status == UserStatus.Suspended)
        {
            return Result<LoginResponse>.Failure(UserErrors.AccountSuspended);
        }

        if (userExists.Status == UserStatus.Deactivated)
        {
            return Result<LoginResponse>.Failure(UserErrors.AccountDeactivated);
        }

        var token = _tokenService.GenerateToken(userExists);

        var response = new LoginResponse(
            token,
            userExists.Id,
            userExists.FirstName,
            userExists.LastName,
            userExists.Email,
            userExists.Role.ToString()
        );

        return Result<LoginResponse>.Success(response);
    }
}