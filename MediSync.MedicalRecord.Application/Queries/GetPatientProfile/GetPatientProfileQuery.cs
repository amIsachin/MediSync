using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Application.DTOs;

namespace MediSync.MedicalRecord.Application.Queries.GetPatientProfile;

public record GetPatientProfileQuery(Guid patientId) : IRequest<Result<PatientProfileDto>>;
