using MediSync.AI.Presentation.Models;
using MediSync.AI.Presentation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.AI.Presentation.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class DrugInteractionController : ControllerBase
    {
        private readonly IDrugInteractionService _drugInteractionService;
        private readonly ILogger<DrugInteractionController> _logger;

        public DrugInteractionController(IDrugInteractionService drugInteractionService, ILogger<DrugInteractionController> logger)
        {
            _drugInteractionService = drugInteractionService;
            _logger = logger;
        }

        [HttpPost("drug-interaction")]
        public async Task<IActionResult> CheckDrugInteraction([FromBody] DrugInteractionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking drug interaction for patient {PatientId} — Drug: {Drug}", request.PatientId, request.NewDrugName);

            var result = await _drugInteractionService.CheckInteractionAsync(request, cancellationToken);

            // Critical interaction → block prescription → HTTP 409
            if (!result.IsSafeToDispense)
            {
                return Conflict(new
                {
                    Blocked = true,
                    result.Severity,
                    result.Interactions,
                    result.Recommendation
                });
            }

            // Safe or minor → approve → HTTP 200
            return Ok(new
            {
                Blocked = false,
                result.Severity,
                result.Interactions,
                result.Recommendation,
                result.IsSafeToDispense
            });

        }
    }
}
