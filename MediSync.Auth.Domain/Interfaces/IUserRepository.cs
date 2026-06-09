using MediSync.Auth.Domain.Aggregates;

namespace MediSync.Auth.Domain.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    public Task AddAsync(User user, CancellationToken cancellationToken = default);

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
