namespace MediSync.AI.Presentation.Models;

public record DrugInteractionRequest(
    Guid PatientId,
    string NewDrugName,
    string NewDosage,
    string Frequency,
    string Route,
    List<ActiveMedication> ActiveMedications,
    List<PatientAllergy> Allergies
);

public record ActiveMedication(
    string DrugName,
    string Dosage,
    string Frequency
);

public record PatientAllergy(
    string Substance,
    string Severity
);