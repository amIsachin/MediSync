namespace MediSync.Auth.Application.Abstractions;

public interface IIdentityService
{
    Task<IdentityCreationResult> CreateUserAsync(Guid domainUserId, string firstName, string lastName, string email, string password, string role, CancellationToken cancellationToken);
}

public record IdentityCreationResult(
    bool Succeeded,
    string? ErrorCode,
    string? ErrorMessage
);