namespace MediSync.MedicalRecord.Application.DTOs;

public record AllergyDto
    (
        Guid Id,
        string Substance,
        string Severity,
        string? Notes,
        DateTime RecordedAt
    );
