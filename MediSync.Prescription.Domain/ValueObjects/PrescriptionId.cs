using MediSync.BuildingBlocks.Domain;
using System.Diagnostics.CodeAnalysis;

namespace MediSync.Prescription.Domain.ValueObjects;

public class PrescriptionId : ValueObject
{
    public Guid Value { get; }

    private PrescriptionId(Guid value)
    {
        Value = value;
    }

    public static PrescriptionId New() => new(Guid.NewGuid());

    public static PrescriptionId From([NotNull] Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new PrescriptionId(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
