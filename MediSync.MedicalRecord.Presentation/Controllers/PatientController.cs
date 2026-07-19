using MediatR;
using MediSync.MedicalRecord.Application.Commands.AddAllergy;
using MediSync.MedicalRecord.Application.Commands.AddDiagnosis;
using MediSync.MedicalRecord.Application.Commands.CreatePatientProfile;
using MediSync.MedicalRecord.Application.Commands.RecordEncounter;
using MediSync.MedicalRecord.Application.Queries.GetPatientByUserId;
using MediSync.MedicalRecord.Application.Queries.GetPatientProfile;
using MediSync.MedicalRecord.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.MedicalRecord.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateProfile")]
        public async Task<IActionResult> CreateProfile([FromBody] CreatePatientProfileRequest request, CancellationToken cancellationToken)
        {
            var command = new CreatePatientProfileCommand(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.BloodGroup,
                request.Gender,
                request.Email,
                request.PhoneNumber
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Message });
            }

            return Created($"/api/patients/{result.Value}", new { PatientId = result.Value });
        }

        [HttpPost("GetById/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetPatientProfileQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(new { result.Error.Code, result.Error.Message });
            }

            return Ok(result.Value);
        }

        [HttpPost("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetPatientByUserIdQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(new { result.Error.Code, result.Error.Message });
            }

            return Ok(result.Value);
        }

        [HttpPost("{id:guid}/allergies")]
        public async Task<IActionResult> AddAllergy(Guid id, [FromBody] AddAllergyRequest request, CancellationToken cancellationToken)
        {
            var command = new AddAllergyCommand(
                id,
                request.Substance,
                request.Severity,
                request.DoctorId,
                request.Notes
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return NotFound(new { result.Error.Code, result.Error.Message });
            }

            return Ok(result.Value);

        }

        [HttpPost("{id:guid}/diagnoses")]
        public async Task<IActionResult> AddDiagnosis(Guid id, [FromBody] AddDiagnosisRequest request, CancellationToken cancellationToken)
        {
            var command = new AddDiagnosisCommand(
                id,
                request.DoctorId,
                request.IcdCode,
                request.Description
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Message });
            }

            return Ok(new { Message = "Diagnosis recorded successfully" });
        }

        [HttpPost("{id:guid}/encounters")]
        public async Task<IActionResult> RecordEncounter(Guid id, [FromBody] RecordEncounterRequest request, CancellationToken cancellationToken)
        {
            var command = new RecordEncounterCommand(
                id,
                request.DoctorId,
                request.EncounterType,
                request.ChiefComplaint,
                request.Notes,
                request.Facility
            );

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(new { result.Error.Code, result.Error.Message });
            }

            return Ok(new { Message = "Encounter recorded successfully" });
        }


    }
}


public record CreatePatientProfileRequest(
    Guid UserId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    BloodGroup BloodGroup,
    GenderType Gender,
    string Email,
    string? PhoneNumber
);

public record AddAllergyRequest(
    string Substance,
    AllergySeverity Severity,
    Guid DoctorId,
    string? Notes
);

public record AddDiagnosisRequest(
    Guid DoctorId,
    string IcdCode,
    string Description
);

public record RecordEncounterRequest(
    Guid DoctorId,
    EncounterType EncounterType,
    string ChiefComplaint,
    string? Notes,
    string? Facility
);