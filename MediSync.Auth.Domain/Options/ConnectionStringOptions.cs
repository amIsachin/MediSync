namespace MediSync.Auth.Domain.Options;

/// <summary>
/// Represents the database connection string settings
/// loaded from the <c>ConnectionStrings</c> configuration section.
/// </summary>
public class ConnectionStringOptions
{
    public const string SectionName = "ConnectionStrings";

    public string DefaultConnection { get; set; } = default!;
}
