namespace MediSync.Prescription.Application.Abstractions;

public interface IPatientDataService
{
    public Task<List<PatientAllergyInfo>> GetPatientAllergiesAsync(Guid patientId, CancellationToken cancellationToken = default);
}

public record PatientAllergyInfo(
    string Substance,
    string Severity
);
