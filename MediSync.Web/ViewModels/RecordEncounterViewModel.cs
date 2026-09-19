using System.ComponentModel.DataAnnotations;

namespace MediSync.Web.ViewModels;

public class RecordEncounterViewModel
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = default!;

    [Required(ErrorMessage = "Encounter type is required")]
    public string EncounterType { get; set; } = "InPerson";

    [Required(ErrorMessage = "Chief complaint is required")]
    [MaxLength(500)]
    public string ChiefComplaint { get; set; } = default!;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(200)]
    public string? Facility { get; set; }
}
