namespace MediSync.BuildingBlocks.Common;

/// <summary>
/// Represents the category of an application error.
/// The API layer uses this value to determine the appropriate HTTP status code.
///
/// HTTP Status Code Mapping:
/// - Failure      : 400 Bad Request
/// - Unauthorized : 401 Unauthorized
/// - Forbidden    : 403 Forbidden
/// - NotFound     : 404 Not Found
/// - Conflict     : 409 Conflict
/// </summary>
public enum ErrorType
{
    None = 0,
    Failure = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5
}

/// <summary>
/// Represents an application error.
/// Contains an error code, a descriptive message, and an error type.
/// </summary>
public sealed class Error
{
    public string Code { get; set; }

    public string Message { get; set; }

    public ErrorType Type { get; set; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }
    
    // Static factory methods — clean API for creating errors
    // Usage: Error.NotFound("User.NotFound", "User was not found")
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);
    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);
    public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

    // None = no error occurred — used as default in Result<T>
    // Empty strings because there is nothing to say when there is no error
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}