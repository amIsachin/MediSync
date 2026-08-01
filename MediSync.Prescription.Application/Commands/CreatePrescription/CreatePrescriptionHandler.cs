using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Domain.Interfaces;
using MediSync.Prescription.Domain.ValueObjects;

namespace MediSync.Prescription.Application.Commands.CreatePrescription;

public class CreatePrescriptionHandler : IRequestHandler<CreatePrescriptionCommand, Result<Guid>>
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public CreatePrescriptionHandler(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<Result<Guid>> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var drug = DrugInfo.Create(request.DrugName, request.Dosage, request.GenericName);
        var dosage = DosageInstruction.Create(request.Frequency, request.Route, request.DurationDays, request.SpecialInstructions);

        var prescription = MediSync.Prescription.Domain.Aggregates.Prescription.Create(request.PatientId, request.DoctorId, drug, dosage, request.Notes);

        await _prescriptionRepository.AddAsync(prescription);

        return Result<Guid>.Success(prescription.Id);
    }
}
