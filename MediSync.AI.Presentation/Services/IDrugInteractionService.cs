using MediSync.AI.Presentation.Models;

namespace MediSync.AI.Presentation.Services;

public interface IDrugInteractionService
{
    public Task<DrugInteractionResult> CheckInteractionAsync(DrugInteractionRequest request, CancellationToken cancellationToken);
}
