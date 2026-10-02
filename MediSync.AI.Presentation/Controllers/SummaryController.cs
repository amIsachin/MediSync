using MediSync.AI.Presentation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.AI.Presentation.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryService _summaryService;
        private readonly ILogger<SummaryController> _logger;

        public SummaryController(ISummaryService summaryService, ILogger<SummaryController> logger)
        {
            _summaryService = summaryService;
            _logger = logger;
        }

        [HttpGet("summary/{patientId:guid}")]
        public async Task<IActionResult> GetSummary(Guid patientId, [FromQuery] string patientName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(patientName))
            {
                return BadRequest(new { Message = "Patient name is required" });
            }

            var response = await _summaryService.SummarizeAsync(patientId, patientName, cancellationToken);

            if (!response.HasData)
            {
                return NotFound(new { Message = response.Summary });
            }

            return Ok(response);
        }
    }
}
