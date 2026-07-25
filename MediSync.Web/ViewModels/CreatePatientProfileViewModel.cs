using System.ComponentModel.DataAnnotations;

namespace MediSync.Web.ViewModels;

public class CreatePatientProfileViewModel
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = default!;

    [Required(ErrorMessage = "Date of birth is required")]
    public string DateOfBirth { get; set; } = default!;

    [Required(ErrorMessage = "Blood group is required")]
    public string BloodGroup { get; set; } = default!;

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = default!;

    public string? PhoneNumber { get; set; }
}
