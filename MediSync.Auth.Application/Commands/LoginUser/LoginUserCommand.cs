using MediatR;
using MediSync.BuildingBlocks.Common;

namespace MediSync.Auth.Application.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

// Response returned on successful login
public record LoginResponse(
    string Token,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Role
);
