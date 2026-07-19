using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.ValueObjects;

/// <summary>
/// Represents an ICD-10 (International Classification of Diseases) code.
/// This value object ensures that diagnosis codes are stored in a
/// consistent and validated format.
/// Example codes: J18.9 = Pneumonia, E11 = Type 2 Diabetes, I10 = Hypertension
/// </summary>
public sealed class IcdCode : ValueObject
{
    public string Value { get; }

    private IcdCode(string value) => Value = value;

    public static IcdCode Create(string code)
    {
        ArgumentException.ThrowIfNullOrEmpty(code, nameof(code));

        // Basic format validation — letter followed by digits
        var trimmed = code.Trim().ToUpperInvariant();

        if (trimmed.Length < 3 || !char.IsLetter(trimmed[0]))
            throw new ArgumentException($"'{code}' is not a valid ICD-10 code.");

        return new IcdCode(trimmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Value;
    }
}
