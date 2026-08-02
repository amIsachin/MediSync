using MediSync.Web.Services.ServiceResponse;

namespace MediSync.Web.IService;

public interface IPrescriptionService
{
    public Task<ServiceResponseMessage<List<PrescriptionResponse>>> GetPatientPrescriptionsAsync(Guid patientId, bool isActiveOnly = false);

    public Task<ServiceResponseMessage<Guid>> CreatePrescriptionAsync(CreatePrescriptionRequest request);

    public Task<ServiceResponseMessage<bool>> CancelPrescriptionAsync(Guid prescriptionId, string reason);
}

public record PrescriptionResponse(
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
    string? Notes
);

public record CreatePrescriptionRequest(
    Guid PatientId,
    Guid DoctorId,
    string DrugName,
    string Dosage,
    string? GenericName,
    string Frequency,
    string Route,
    int DurationDays,
    string? SpecialInstructions,
    string? Notes
);