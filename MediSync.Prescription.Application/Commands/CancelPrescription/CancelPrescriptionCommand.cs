using MediatR;
using MediSync.BuildingBlocks.Common;

namespace MediSync.Prescription.Application.Commands.CancelPrescription;

public record CancelPrescriptionCommand(
    Guid PrescriptionId,
    string Reason
) : IRequest<Result<bool>>;