namespace MediSync.Auth.Application.Abstractions;

public interface IAuthenticationService
{
    Task<SignInCheckResult> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken);
}

public record SignInCheckResult(
    bool Succeeded,
    bool IsLockedOut,
    Guid DomainUserId
);