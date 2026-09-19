namespace MediSync.AI.Presentation.Models;

public record IndexPatientRequest(
    Guid PatientId,
    List<AllergyRecord> Allergies,
    List<DiagnosisRecord> Diagnoses,
    List<EncounterRecord> Encounters,
    List<PrescriptionRecord> Prescriptions
);

public record AllergyRecord(
    string Substance,
    string Severity,
    string? Notes,
    DateTime RecordedAt
);

public record DiagnosisRecord(
    string IcdCode,
    string Description,
    Guid DoctorId,
    DateTime DiagnosedAt,
    bool IsActive
);

public record EncounterRecord(
    Guid DoctorId,
    DateTime VisitDate,
    string EncounterType,
    string ChiefComplaint,
    string? Notes,
    string? Facility
);

public record PrescriptionRecord(
    string DrugName,
    string Dosage,
    string Frequency,
    string Route,
    int DurationDays,
    string Status,
    DateTime ExpiresAt,
    string? Notes
);
