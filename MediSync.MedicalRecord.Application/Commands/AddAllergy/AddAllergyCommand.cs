using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Domain.Enums;

namespace MediSync.MedicalRecord.Application.Commands.AddAllergy;

public record AddAllergyCommand
    (
        Guid PatientId,
        string Substance,
        AllergySeverity Severity,
        Guid DoctorId,
        string? Notes = null
    ) : IRequest<Result<Guid>>;
