using MediSync.BuildingBlocks.Domain;

namespace MediSync.Prescription.Domain.ValueObjects;

public class DrugInfo : ValueObject
{
    public string DrugName { get; } = default!;

    public string Dosage { get; } = default!;

    public string? GenericName { get; }

    private DrugInfo(string drugName, string dosage, string? genericName)
    {
        DrugName = drugName;
        Dosage = dosage;
        GenericName = genericName;
    }

    public static DrugInfo Create(string drugName, string dosage, string? genericName = null)
    {
        if (string.IsNullOrWhiteSpace(drugName))
        {
            throw new ArgumentException("Drug name cannot be null or empty.", nameof(drugName));
        }
        if (string.IsNullOrWhiteSpace(dosage))
        {
            throw new ArgumentException("Dosage cannot be null or empty.", nameof(dosage));
        }
        if (string.IsNullOrWhiteSpace(genericName))
        {
            throw new ArgumentException("Generic name cannot be null or empty.", nameof(genericName));
        }

        return new DrugInfo(drugName, dosage, genericName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DrugName.ToLowerInvariant();
        yield return Dosage.ToLowerInvariant();
    }

    public override string ToString() => $"{DrugName} {Dosage}" + (string.IsNullOrWhiteSpace(GenericName) ? "" : $" ({GenericName})");
}
