namespace MediSync.Prescription.Domain.Interfaces;

public interface IPrescriptionRepository
{
    Task<MediSync.Prescription.Domain.Aggregates.Prescription> GetByIdAsync(Guid prescriptionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MediSync.Prescription.Domain.Aggregates.Prescription>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MediSync.Prescription.Domain.Aggregates.Prescription>> GetActiveByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);

    Task AddAsync(MediSync.Prescription.Domain.Aggregates.Prescription prescription, CancellationToken cancellationToken = default);

    Task UpdateAsync(MediSync.Prescription.Domain.Aggregates.Prescription prescription, CancellationToken cancellationToken = default);
}
