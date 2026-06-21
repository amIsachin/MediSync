using MediatR;
using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Domain.Aggregates;
using MediSync.Auth.Domain.Errors;
using MediSync.Auth.Domain.Interfaces;
using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.BuildingBlocks.Common;

namespace MediSync.Auth.Application.Commands.LoginUser;

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
            userExists.Role.ToString());

        //return Result<LoginResponse>.Success(new LoginResponse(token, userExists.Id, userExists.FirstName, userExists.LastName, userExists.Email, userExists.Role.ToString()));

        return Result<LoginResponse>.Success(response);

    }
}