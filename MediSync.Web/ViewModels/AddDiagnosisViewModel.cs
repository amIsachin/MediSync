using System.ComponentModel.DataAnnotations;

namespace MediSync.Web.ViewModels;

public class AddDiagnosisViewModel
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = default!;

    [Required(ErrorMessage = "ICD Code is required")]
    [MaxLength(10)]
    public string IcdCode { get; set; } = default!;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(500)]
    public string Description { get; set; } = default!;
}
