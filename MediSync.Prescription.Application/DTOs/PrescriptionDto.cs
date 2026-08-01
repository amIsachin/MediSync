namespace MediSync.Prescription.Application.DTOs;

public record PrescriptionDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    string DrugName,
    string Dosage,
    string? GenericName,
    string Frequency,
    string Route,
    int DurationDays,
    string? SpecialInstructions,
    string Status,
    DateTime PrescribedAt,
    DateTime ExpiresAt,
    string? Notes,
    Guid? SupersededById
);
