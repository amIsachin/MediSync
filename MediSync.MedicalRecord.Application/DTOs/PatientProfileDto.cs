namespace MediSync.MedicalRecord.Application.DTOs;

public record   PatientProfileDto
    (
        Guid Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string FullName,
        DateOnly DateOfBirth,
        int Age,
        string BloodGroup,
        string Gender,
        string Email,
        string? PhoneNumber,
        string Status,
        List<AllergyDto> Allergies,
        List<DiagnosisDto> Diagnoses,
        List<EncounterDto> Encounters
    );