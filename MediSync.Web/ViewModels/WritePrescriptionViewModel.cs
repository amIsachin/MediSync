using System.ComponentModel.DataAnnotations;

namespace MediSync.Web.ViewModels;

public class WritePrescriptionViewModel
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = default!;

    [Required(ErrorMessage = "Drug name is required")]
    public string DrugName { get; set; } = default!;

    [Required(ErrorMessage = "Dosage is required")]
    public string Dosage { get; set; } = default!;

    public string? GenericName { get; set; }

    [Required]
    public string Frequency { get; set; } = "OnceDaily";

    [Required]
    public string Route { get; set; } = "Oral";

    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int DurationDays { get; set; } = 7;

    public string? SpecialInstructions { get; set; }
    public string? Notes { get; set; }
}
