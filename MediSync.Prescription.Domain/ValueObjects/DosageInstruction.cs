using MediSync.BuildingBlocks.Domain;
using MediSync.Prescription.Domain.Enums;

namespace MediSync.Prescription.Domain.ValueObjects;

public class DosageInstruction : ValueObject
{
    public FrequencyType Frequency { get; }

    public RouteOfAdministration Route { get; }

    public int DurationDays { get; set; }

    public string? SpecialInstructions { get; set; }    // e.g. "take with food"

    public DosageInstruction(FrequencyType frequency, RouteOfAdministration route, int durationDays, string? specialInstructions = null)
    {
        Frequency = frequency;
        Route = route;
        DurationDays = durationDays;
        SpecialInstructions = specialInstructions;
    }

    public static DosageInstruction Create(FrequencyType frequency, RouteOfAdministration route, int durationDays, string? specialInstructions = null)
    {
        if (durationDays < 1 && durationDays > 365)
        {
            throw new ArgumentException("Duration must be between 1 and 365 days.", nameof(durationDays));
        }

        return new DosageInstruction(frequency, route, durationDays, specialInstructions);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Frequency;
        yield return Route;
        yield return DurationDays;
    }

    override public string ToString()
    {
        return $"{Frequency} via {Route} for {DurationDays} days";
    }
}
