using MediSync.Auth.Domain.Aggregates;

namespace MediSync.Auth.Application.Abstractions;

public interface ITokenService
{
    string GenerateToken(User user);
}
