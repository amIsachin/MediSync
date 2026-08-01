using FluentValidation;

namespace MediSync.Prescription.Application.Commands.CreatePrescription;

public class CreatePrescriptionValidator : AbstractValidator<CreatePrescriptionCommand>
{
    public CreatePrescriptionValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required");

        RuleFor(x => x.DrugName)
            .NotEmpty().WithMessage("Drug name is required")
            .MaximumLength(200).WithMessage("Drug name cannot exceed 200 characters");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required")
            .MaximumLength(50).WithMessage("Dosage cannot exceed 50 characters");

        RuleFor(x => x.DurationDays)
            .InclusiveBetween(1, 365)
            .WithMessage("Duration must be between 1 and 365 days");

        RuleFor(x => x.Frequency)
            .IsInEnum().WithMessage("Invalid frequency");

        RuleFor(x => x.Route)
            .IsInEnum().WithMessage("Invalid route of administration");
    }
}
