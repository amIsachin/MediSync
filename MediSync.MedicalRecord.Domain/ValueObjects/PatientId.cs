using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.ValueObjects;

public sealed class PatientId : ValueObject
{
    public Guid Value { get; }

    private PatientId(Guid value)
    {
        Value = value;
    }

    public static PatientId New()
    {
        return new PatientId(Guid.NewGuid());
    }

    public static PatientId From(Guid value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value.ToString(), nameof(value));

        return new PatientId(value);
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