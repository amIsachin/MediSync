using MediSync.Prescription.Domain.Enums;
using MediSync.Prescription.Domain.Interfaces;
using MediSync.Prescription.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Prescription.Infrastructure.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly PrescriptionDbContext _dbContext;

    public PrescriptionRepository(PrescriptionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Domain.Aggregates.Prescription prescription, CancellationToken cancellationToken = default)
    {
        await _dbContext.Prescriptions.AddAsync(prescription, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Aggregates.Prescription>> GetActiveByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var prescriptions = await _dbContext.Prescriptions.Where(p => p.PatientId == patientId && p.Status == PrescriptionStatus.Active).OrderByDescending(o => o.PrescribedAt).ToListAsync(cancellationToken);
        return prescriptions.AsReadOnly();
    }

    public async Task<Domain.Aggregates.Prescription> GetByIdAsync(Guid prescriptionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Prescriptions.FirstOrDefaultAsync(p => p.Id == prescriptionId, cancellationToken) ?? null!;
    }

    public async Task<IReadOnlyList<Domain.Aggregates.Prescription>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var prescriptions = await _dbContext.Prescriptions.Where(p => p.PatientId == patientId).OrderByDescending(p => p.PrescribedAt).ToListAsync(cancellationToken);
        return prescriptions.AsReadOnly();
    }

    public async Task UpdateAsync(Domain.Aggregates.Prescription prescription, CancellationToken cancellationToken = default)
    {
        _dbContext.Prescriptions.Update(prescription);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
