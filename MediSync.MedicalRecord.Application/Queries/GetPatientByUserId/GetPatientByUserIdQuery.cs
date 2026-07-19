using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Application.DTOs;

namespace MediSync.MedicalRecord.Application.Queries.GetPatientByUserId;

public record GetPatientByUserIdQuery(Guid userId) : IRequest<Result<PatientProfileDto>>;