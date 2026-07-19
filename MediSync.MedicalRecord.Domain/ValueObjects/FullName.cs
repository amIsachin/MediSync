using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.ValueObjects;

public sealed class FullName : ValueObject
{
    public string FirstName { get; } = default!;

    public string LastName { get; } = default!;

    public string DisplayName => $"{FirstName} {LastName}";

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static FullName Create(string firstName, string lastName)
    {
        ArgumentNullException.ThrowIfNull(firstName, nameof(firstName));
        ArgumentNullException.ThrowIfNull(lastName, nameof(lastName));

        return new FullName(firstName.Trim(), lastName.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString()
    {
        return DisplayName;
    }
}
