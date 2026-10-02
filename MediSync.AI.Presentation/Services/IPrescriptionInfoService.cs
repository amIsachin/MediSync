using MediSync.AI.Presentation.Models;

namespace MediSync.AI.Presentation.Services;

public interface IPrescriptionInfoService
{
    Task<PrescriptionInfoResponse> GetInfoAsync(PrescriptionInfoRequest request, CancellationToken cancellationToken = default);
}