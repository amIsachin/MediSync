using MediSync.MedicalRecord.Domain.Aggregates;
using MediSync.MedicalRecord.Domain.Entities;
using MediSync.MedicalRecord.Domain.Interfaces;
using MediSync.MedicalRecord.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.MedicalRecord.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly MedicalRecordDbContext _dbContext;

    public PatientRepository(MedicalRecordDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(patient, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbContext.Patients.AnyAsync(p => p.UserId == userId, cancellationToken);

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.Patients
        .Include("Allergies")
        .Include("Diagnoses")
        .Include("Encounters").FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbContext.Patients
        .Include("Allergies")
        .Include("Diagnoses")
        .Include("Encounters").FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        foreach (var entry in _dbContext.ChangeTracker.Entries())
        {
            var clrType = entry.Metadata.ClrType;

            if ((clrType == typeof(Allergy) ||
                 clrType == typeof(Diagnosis) ||
                 clrType == typeof(Encounter)) &&
                 entry.State == EntityState.Modified)
            {
                // These are newly added child entities
                // EF Core incorrectly marks them as Modified
                // Force them to Added so EF Core does INSERT not UPDATE
                entry.State = EntityState.Added;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
