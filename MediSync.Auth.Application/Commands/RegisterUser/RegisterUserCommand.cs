using MediSync.Auth.Domain.Utilities.Enums;
using MediSync.BuildingBlocks.Common;

namespace MediSync.Auth.Application.Commands.RegisterUser;

public record RegisterUserCommand(string firstName, string lastName, string email, string password, UserRole role) : MediatR.IRequest<Result<Guid>>;