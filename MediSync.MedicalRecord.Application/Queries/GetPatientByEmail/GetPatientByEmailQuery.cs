using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Application.DTOs;

namespace MediSync.MedicalRecord.Application.Queries.GetPatientByEmail;

public record GetPatientByEmailQuery(string Email) : IRequest<Result<PatientProfileDto>>;
