namespace MediSync.MedicalRecord.Application.DTOs;

public record EncounterDto
    (
        Guid Id,
        Guid DoctorId,
        string EncounterType,
        string ChiefComplaint,
        string? Notes,
        string? Facility,
        DateTime VisitDate
    );
