using FluentValidation;

namespace MediSync.Prescription.Application.Commands.SupersedePrescription;

public class SupersedePrescriptionValidator : AbstractValidator<SupersedePrescriptionCommand>
{
    public SupersedePrescriptionValidator()
    {
        RuleFor(x => x.OldPrescriptionId)
            .NotEmpty().WithMessage("OldPrescriptionId is required");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required");

        RuleFor(x => x.DrugName)
            .NotEmpty().WithMessage("Drug name is required");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required");

        RuleFor(x => x.DurationDays)
            .InclusiveBetween(1, 365)
            .WithMessage("Duration must be between 1 and 365 days");
    }
}
