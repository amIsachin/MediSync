using FluentValidation;

namespace MediSync.Prescription.Application.Commands.CancelPrescription;

public  class CancelPrescriptionValidator : AbstractValidator<CancelPrescriptionCommand>
{
    public CancelPrescriptionValidator()
    {
        RuleFor(x => x.PrescriptionId)
            .NotEmpty().WithMessage("PrescriptionId is required");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Cancellation reason is required")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");
    }
}
