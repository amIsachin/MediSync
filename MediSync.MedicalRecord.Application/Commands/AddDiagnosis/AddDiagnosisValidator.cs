using FluentValidation;

namespace MediSync.MedicalRecord.Application.Commands.AddDiagnosis;

public class AddDiagnosisValidator : AbstractValidator<AddDiagnosisCommand>
{
    public AddDiagnosisValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required");

        RuleFor(x => x.IcdCode)
            .NotEmpty().WithMessage("ICD code is required")
            .MaximumLength(10).WithMessage("ICD code cannot exceed 10 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}
