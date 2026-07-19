using MediatR;
using MediSync.BuildingBlocks.Common;

namespace MediSync.MedicalRecord.Application.Commands.AddDiagnosis;

public record AddDiagnosisCommand
    (
        Guid PatientId,
        Guid DoctorId,
        string IcdCode,
        string Description
    ) : IRequest<Result<Guid>>;
