using MediSync.MedicalRecord.Domain.Aggregates;

namespace MediSync.MedicalRecord.Domain.Interfaces;

public interface IPatientRepository
{
    // Get by domain ID
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Get by Auth.API UserId — used when patient logs in
    Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Check if profile already exists for this user
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Save new patient
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);

    // Save changes to existing patient
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
}
