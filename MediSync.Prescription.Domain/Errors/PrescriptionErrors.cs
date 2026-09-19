using MediSync.BuildingBlocks.Common;

namespace MediSync.Prescription.Domain.Errors;

public sealed class PrescriptionErrors
{
    public static Error NotFound(Guid id) =>
         Error.NotFound("Prescription.NotFound", $"Prescription with ID {id} was not found");

    public static readonly Error AlreadyCancelled =
        Error.Failure("Prescription.AlreadyCancelled", "This prescription has already been cancelled");

    public static readonly Error AlreadySuperseded =
        Error.Failure("Prescription.AlreadySuperseded", "This prescription has already been superseded");

    public static readonly Error AlreadyExpired =
        Error.Failure("Prescription.AlreadyExpired", "This prescription has already expired");

    public static readonly Error CannotModifyActive =
        Error.Failure("Prescription.CannotModify", "An active prescription cannot be modified — create a new one");

    public static Error DrugInteraction(IEnumerable<string> interactions, string recommendation) =>
        Error.Conflict("Prescription.DrugInteraction", $"Prescription blocked — {string.Join(", ", interactions)}. {recommendation}");

}
