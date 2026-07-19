using MediSync.BuildingBlocks.Domain;
using MediSync.MedicalRecord.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace MediSync.MedicalRecord.Infrastructure.Persistence;

public class MedicalRecordDbContext(DbContextOptions<MedicalRecordDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Auto-discovers all IEntityTypeConfiguration classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MedicalRecordDbContext).Assembly);
    }

    // Override SaveChangesAsync to publish domain events
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect aggregates with events BEFORE saving
        var aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>().Where(e => e.Entity.DomainEvents.Any()).Select(e => e.Entity).ToList();

        // Save to database first
        var result = await base.SaveChangesAsync(cancellationToken);

        // Publish events AFTER successful save
        if (result > 0)
        {
            foreach (var aggregate in aggregatesWithEvents)
            {
                foreach (var domainEvent in aggregate.DomainEvents)
                {
                    // TODO: _eventBus.PublishAsync(domainEvent)
                    // For now — log to console so you can see events firing
                    Console.WriteLine($"Domain Event: {domainEvent.GetType().Name}");
                }

                aggregate.ClearDomainEvents();
            }
        }

        return result;
    }
}
