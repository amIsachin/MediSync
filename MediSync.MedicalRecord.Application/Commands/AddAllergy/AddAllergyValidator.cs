using FluentValidation;

namespace MediSync.MedicalRecord.Application.Commands.AddAllergy;

public class AddAllergyValidator : AbstractValidator<AddAllergyCommand>
{
    public AddAllergyValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required");

        RuleFor(x => x.Substance)
            .NotEmpty().WithMessage("Substance is required")
            .MaximumLength(200).WithMessage("Substance cannot exceed 200 characters");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required");

        RuleFor(x => x.Severity)
            .IsInEnum().WithMessage("Invalid severity value");
    }
}