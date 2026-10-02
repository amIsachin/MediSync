namespace MediSync.Prescription.Application.Abstractions;

public interface IAIPrescriptionIndexService
{
    Task IndexPrescriptionsAsync(Guid patientId, CancellationToken cancellationToken = default);
}
