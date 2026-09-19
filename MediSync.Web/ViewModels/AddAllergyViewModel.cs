using System.ComponentModel.DataAnnotations;

namespace MediSync.Web.ViewModels;

public class AddAllergyViewModel
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = default!;

    [Required(ErrorMessage = "Substance is required")]
    [MaxLength(200)]
    public string Substance { get; set; } = default!;

    [Required(ErrorMessage = "Severity is required")]
    public string Severity { get; set; } = "Mild";

    public string? Notes { get; set; }
}
