using MediSync.AI.Presentation.Models;

namespace MediSync.AI.Presentation.Services;

public interface IIndexService
{
    Task IndexPatientAsync(IndexPatientRequest request, CancellationToken cancellationToken = default);
}
