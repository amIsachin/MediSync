using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Domain.Enums;

namespace MediSync.MedicalRecord.Application.Commands.RecordEncounter;

public record RecordEncounterCommand
    (
        Guid PatientId,
        Guid DoctorId,
        EncounterType EncounterType,
        string ChiefComplaint,
        string? Notes = null,
        string? Facility = null
    ) : IRequest<Result<Guid>>;
