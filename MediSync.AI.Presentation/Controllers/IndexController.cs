using MediSync.AI.Presentation.Models;
using MediSync.AI.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.AI.Presentation.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly IIndexService _indexService;
        private readonly ILogger<IndexController> _logger;

        public IndexController(IIndexService indexService, ILogger<IndexController> logger)
        {
            _indexService = indexService;
            _logger = logger;
        }

        [HttpPost("index")]
        public async Task<IActionResult> IndexPatient([FromBody] IndexPatientRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Index request received for patient {PatientId}", request.PatientId);

            await _indexService.IndexPatientAsync(request, cancellationToken);

            return Ok(new
            {
                Message = "Patient records indexed successfully",
                PatientId = request.PatientId,
                Allergies = request.Allergies.Count,
                Diagnoses = request.Diagnoses.Count,
                Encounters = request.Encounters.Count,
                Prescriptions = request.Prescriptions.Count
            });
        }
    }
}
