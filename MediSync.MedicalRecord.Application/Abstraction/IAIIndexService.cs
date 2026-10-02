namespace MediSync.MedicalRecord.Application.Abstraction;

public interface IAIIndexService
{
    Task IndexPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
}
