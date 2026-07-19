using FluentValidation;

namespace MediSync.MedicalRecord.Application.Commands.RecordEncounter;

public class RecordEncounterValidator : AbstractValidator<RecordEncounterCommand>
{
    public RecordEncounterValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required");

        RuleFor(x => x.EncounterType)
            .IsInEnum().WithMessage("Invalid encounter type");

        RuleFor(x => x.ChiefComplaint)
            .NotEmpty().WithMessage("Chief complaint is required")
            .MaximumLength(500).WithMessage("Chief complaint cannot exceed 500 characters");
    }
}
