using MediSync.Web.IService;

namespace MediSync.Web.ViewModels;

public class PatientDetailViewModel
{
    public PatientProfileResponse Profile { get; set; } = default!;
    public List<PrescriptionResponse> Prescriptions { get; set; } = new();
}
