namespace MediSync.Prescription.Infrastructure.Services;

public interface IAIService
{
    public Task<DrugInteractionCheckResult> CheckDrugInteractionAsync(DrugInteractionCheckRequest request, CancellationToken cancellationToken);
}

public record DrugInteractionCheckRequest(
    Guid PatientId,
    string NewDrugName,
    string NewDosage,
    string Frequency,
    string Route,
    List<ActiveMedicationInfo> ActiveMedications,
    List<AllergyInfo> Allergies
);

public record ActiveMedicationInfo(
    string DrugName,
    string Dosage,
    string Frequency
);

public record AllergyInfo(
    string Substance,
    string Severity
);

public record DrugInteractionCheckResult(
    bool Blocked,
    string Severity,
    List<string> Interactions,
    string Recommendation,
    bool IsSafeToDispense
);