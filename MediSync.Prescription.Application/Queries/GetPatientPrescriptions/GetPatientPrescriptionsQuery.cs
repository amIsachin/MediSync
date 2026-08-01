using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Application.DTOs;

namespace MediSync.Prescription.Application.Queries.GetPatientPrescriptions;

public record GetPatientPrescriptionsQuery(
    Guid PatientId,
    bool ActiveOnly = false
) : IRequest<Result<List<PrescriptionDto>>>;
