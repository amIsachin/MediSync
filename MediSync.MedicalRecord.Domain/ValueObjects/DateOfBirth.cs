using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.ValueObjects;

public sealed class DateOfBirth : ValueObject
{
    public DateOnly Value { get; }

    // Computed — no storage needed
    public int Age => CalculateAge();

    private DateOfBirth(DateOnly value) => Value = value;

    public static DateOfBirth From(DateOnly date)
    {
        if (date >= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Date of birth cannot be in the future.");

        if (date.Year < 1900)
            throw new ArgumentException("Date of birth is not valid.");

        return new DateOfBirth(date);
    }

    private int CalculateAge()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - Value.Year;

        // Adjust if birthday has not occurred yet this year
        if (Value > today.AddYears(-age)) age--;

        return age;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("dd MMM yyyy");
}
