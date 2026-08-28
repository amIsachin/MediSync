using MediSync.BuildingBlocks.Common;

namespace MediSync.Notification.Presentation.Services;

public interface IPatientService
{
    public Task<Result<PatientContactInfo>> GetPatientContactAsync(Guid patientId, CancellationToken cancellationToken);
}

public record PatientContactInfo(
    Guid Id,
    string FullName,
    string Email   
);
