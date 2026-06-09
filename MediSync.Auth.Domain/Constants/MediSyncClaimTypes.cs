namespace MediSync.Auth.Domain.Constants;

// static class — just constants, no instances needed
public static class MediSyncClaimTypes
{
    // The user's domain ID (from Users table)
    // Every service reads this to know who is making the request
    public const string UserID = "medisync:user_id";

    // The user's role — "Patient", "Doctor", "Admin", etc.
    // Every service reads this for authorization decisions
    public const string Role = "medisync:role";

    // Only present in token when Role = Patient
    // Prescription.API reads this to know which patient is logged in
    public const string PatientId = "medisync:patient_id";

    // Only present in token when Role = Doctor
    // Lets services know which doctor is making the request
    public const string DoctorId = "medisync:doctor_id";

    // Only present in token when Role = Doctor
    // Coordination.API uses this for doctor matching
    public const string Specialty = "medisync:specialty";

    // The user's current status — Active, Suspended, etc.
    public const string Status = "medisync:status";
}
