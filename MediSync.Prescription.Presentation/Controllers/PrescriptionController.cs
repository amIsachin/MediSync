using MediatR;
using MediSync.Prescription.Application.Commands.CancelPrescription;
using MediSync.Prescription.Application.Commands.CreatePrescription;
using MediSync.Prescription.Application.Commands.SupersedePrescription;
using MediSync.Prescription.Application.Queries.GetPatientPrescriptions;
using MediSync.Prescription.Application.Queries.GetPrescriptionById;
using MediSync.Prescription.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.Prescription.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PrescriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionRequest request, CancellationToken cancellationToken)
        {
            var command = new CreatePrescriptionCommand(
                request.PatientId,
                request.DoctorId,
                request.DrugName,
                request.Dosage,
                request.GenericName,
                request.Frequency,
                request.Route,
                request.DurationDays,
                request.SpecialInstructions,
                request.Notes
            );

            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Message });
            }

            return Created($"/api/prescriptions/{result.Value}", result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetPrescriptionByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(new { result.Error.Code, result.Error.Message });
            }

            return Ok(result.Value);
        }

        [HttpGet("patient/{patientId:guid}")]
        public async Task<IActionResult> GetByPatientId(Guid patientId, [FromQuery] bool activeOnly = false, CancellationToken cancellationToken = default)
        {
            var query = new GetPatientPrescriptionsQuery(patientId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Message });
            }

            return Ok(result.Value);
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelPrescriptionRequest request, CancellationToken cancellationToken)
        {
            var command = new CancelPrescriptionCommand(
                 id,
                 request.Reason
             );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Message });
            }

            return Ok(new { Message = "Prescription cancelled successfully" });
        }

        [HttpPost("{id:guid}/supersede")]
        public async Task<IActionResult> Supersede([FromRoute] Guid id, [FromBody] SupersedePrescriptionRequest request, CancellationToken cancellationToken)
        {
            var command = new SupersedePrescriptionCommand(
                  id,
                  request.PatientId,
                  request.DoctorId,
                  request.DrugName,
                  request.Dosage,
                  request.GenericName,
                  request.Frequency,
                  request.Route,
                  request.DurationDays,
                  request.SpecialInstructions,
                  request.Notes
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Message });
            }

            return Created($"/api/prescriptions/{result.Value}", new { NewPrescriptionId = result.Value });

            //return Created($"/api/prescriptions/{result.Value}", result.Value);
        }
    }
}

public record CreatePrescriptionRequest(
    Guid PatientId,
    Guid DoctorId,
    string DrugName,
    string Dosage,
    string? GenericName,
    FrequencyType Frequency,
    RouteOfAdministration Route,
    int DurationDays,
    string? SpecialInstructions,
    string? Notes
);

public record CancelPrescriptionRequest(string Reason);

public record SupersedePrescriptionRequest(
    Guid PatientId,
    Guid DoctorId,
    string DrugName,
    string Dosage,
    string? GenericName,
    FrequencyType Frequency,
    RouteOfAdministration Route,
    int DurationDays,
    string? SpecialInstructions,
    string? Notes
);
