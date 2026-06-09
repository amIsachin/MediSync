using MediSync.Auth.Domain.Aggregates;
using MediSync.Auth.Infrastrucure.Identity;
using MediSync.BuildingBlocks.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Auth.Infrastrucure.Persistence;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get all modified/added aggregates that have domain events
        var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>().Where(e => e.Entity.DomainEvents.Any()).Select(e => e.Entity).ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var aggregate in aggregatesWithEvents)
        {
            foreach (var item in aggregate.DomainEvents)
            {
                // _eventBus.PublishAsync(domainEvent) — injected via constructor
                // For now — the pattern is in place, implementation follows
            }

            aggregate.ClearDomainEvents();

        }

        return result;
    }
}