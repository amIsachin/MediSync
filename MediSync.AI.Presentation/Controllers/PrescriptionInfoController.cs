using MediSync.AI.Presentation.Models;
using MediSync.AI.Presentation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.AI.Presentation.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class PrescriptionInfoController : ControllerBase
    {
        private readonly IPrescriptionInfoService _prescriptionInfoService;
        private readonly ILogger<PrescriptionInfoController> _logger;

        public PrescriptionInfoController(IPrescriptionInfoService prescriptionInfoService, ILogger<PrescriptionInfoController> logger)
        {
            _prescriptionInfoService = prescriptionInfoService;
            _logger = logger;
        }

        [HttpPost("prescription-info")]
        public async Task<IActionResult> GetPrescriptionInfo([FromBody] PrescriptionInfoRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.DrugName))
            {
                return BadRequest(new { Message = "Drug name is required" });
            }

            var response = await _prescriptionInfoService.GetInfoAsync(request, cancellationToken);

            return Ok(response);
        }
    }
}
