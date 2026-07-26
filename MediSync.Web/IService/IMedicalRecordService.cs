using MediSync.Web.Services.ServiceResponse;

namespace MediSync.Web.IService;

public interface IMedicalRecordService
{
    public Task<ServiceResponseMessage<PatientProfileResponse>> GetPatientByUserIdAsync(Guid userId);
    public Task<ServiceResponseMessage<Guid>> CreatePatientProfileAsync(CreatePatientProfileRequest request);
}

public record PatientProfileResponse(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string FullName,
    string DateOfBirth,
    int Age,
    string BloodGroup,
    string Gender,
    string Email,
    string? PhoneNumber,
    string Status,
    List<AllergyResponse> Allergies,
    List<DiagnosisResponse> Diagnoses,
    List<EncounterResponse> Encounters

//Guid Id,
//Guid userId,
//string FirstName,
//string LastName,
//string FullName,
//string DateOfBirth,
//int Age,
//string BloodGroup,
//string Gender,
//string Email,
//string? PhoneNumber,
//string Status,

//List<AllergyResponse> Allergies,
//List<DiagnosisResponse> Diagnoses,
//List<EncounterResponse> Encounters
);

public record AllergyResponse(
    Guid Id,
    string Substance,
    string Severity,
    string? Notes,
    DateTime RecordedAt
);

public record DiagnosisResponse(
    Guid Id,
    string IcdCode,
    string Description,
    Guid DoctorId,
    DateTime DiagnosedAt,
    bool IsActive
);

public record EncounterResponse(
    Guid Id,
    Guid DoctorId,
    string EncounterType,
    string ChiefComplaint,
    string? Notes,
    string? Facility,
    DateTime VisitDate
);

public record CreatePatientProfileRequest(
    Guid UserId,
    string FirstName,
    string LastName,
    string DateOfBirth,
    string BloodGroup,
    string Gender,
    string Email,
    string? PhoneNumber
);