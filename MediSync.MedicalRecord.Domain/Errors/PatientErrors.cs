using MediSync.BuildingBlocks.Common;

namespace MediSync.MedicalRecord.Domain.Errors;

public static class PatientErrors
{
    public static Error NotFound(Guid id)
        => Error.NotFound("Patient.NotFound", $"Patient with id '{id}' was not found.");

    public static Error AlreadyExists(Guid userId)
        => Error.Conflict("Patient.AlreadyExists", $"Patient with user id '{userId}' already exists.");

    public static readonly Error Deactivated = Error.Failure("Patient.Deactivated", "Cannot modify a deactivated patient record");

    public static Error AllergyAlreadyExists(string substance)
        => Error.Conflict("Patient.AllergyAlreadyExists", $"Allergy with substance '{substance}' already exists for this patient.");

    public static Error DiagnosisNotFound(Guid allergyId)
        => Error.NotFound("Patient.DiagnosisNotFound", $"Diagnosis with id '{allergyId}' was not found for this patient.");

    public static readonly Error Unauthorized = Error.Forbidden("Patient.Unauthorized", "You do not have permission to access this patient's records");
}