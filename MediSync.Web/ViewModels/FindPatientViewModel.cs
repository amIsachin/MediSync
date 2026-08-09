using System.ComponentModel.DataAnnotations;

namespace MediSync.Web.ViewModels;

public class FindPatientViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = default!;
}
