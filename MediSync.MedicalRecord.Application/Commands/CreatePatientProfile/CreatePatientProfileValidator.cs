using FluentValidation;

namespace MediSync.MedicalRecord.Application.Commands.CreatePatientProfile;

public class CreatePatientProfileValidator : AbstractValidator<CreatePatientProfileCommand>
{
    public CreatePatientProfileValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(d => d < DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth cannot be in the future")
            .Must(d => d.Year >= 1900)
                .WithMessage("Date of birth is not valid");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters")
            .When(x => x.PhoneNumber != null);  // only validate if provided

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");
    }
}
