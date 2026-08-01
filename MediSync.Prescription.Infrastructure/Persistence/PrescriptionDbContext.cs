using MediSync.BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Prescription.Infrastructure.Persistence;

public class PrescriptionDbContext(DbContextOptions<PrescriptionDbContext> options) : DbContext(options)
{
    public DbSet<MediSync.Prescription.Domain.Aggregates.Prescription> Prescriptions => Set<MediSync.Prescription.Domain.Aggregates.Prescription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrescriptionDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>().Where(e => e.Entity.DomainEvents.Any()).Select(e => e.Entity).ToList();
        int result = await base.SaveChangesAsync(cancellationToken);

        if (result > 0)
        {
            foreach (var aggregate in aggregatesWithEvents)
            {
                foreach (var domainEvent in aggregate.DomainEvents)
                {
                    Console.WriteLine($"Domain Event: {domainEvent.GetType().Name}");
                }

                aggregate.ClearDomainEvents();
            }
        }

        return result;
    }
}
