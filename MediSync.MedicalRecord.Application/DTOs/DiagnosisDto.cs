namespace MediSync.MedicalRecord.Application.DTOs;

public record DiagnosisDto
    (
        Guid Id,
        string IcdCode,
        string Description,
        Guid DoctorId,
        DateTime DiagnosedAt,
        bool IsActive
    );
