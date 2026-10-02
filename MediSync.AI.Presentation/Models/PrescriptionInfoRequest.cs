namespace MediSync.AI.Presentation.Models;

public record PrescriptionInfoRequest(
    string DrugName,
    string Dosage,
    string? PatientQuestion
);

public record PrescriptionInfoResponse(
    string DrugName,
    string Purpose,
    string HowToTake,
    string CommonSideEffects,
    string WhenToCallDoctor,
    string ImportantWarnings
);
