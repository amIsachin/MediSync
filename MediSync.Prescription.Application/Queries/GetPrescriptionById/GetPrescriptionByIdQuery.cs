using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Application.DTOs;

namespace MediSync.Prescription.Application.Queries.GetPrescriptionById;

public record GetPrescriptionByIdQuery(
    Guid PrescriptionId
) : IRequest<Result<PrescriptionDto>>;
