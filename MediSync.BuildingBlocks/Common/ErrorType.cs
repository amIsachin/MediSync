namespace MediSync.BuildingBlocks.Common;

// ErrorType tells the API layer what HTTP status code to return
// NotFound    = 404
// Conflict    = 409
// Unauthorized= 401
// Forbidden   = 403
// Failure     = 400
public enum ErrorType
{
    None = 0,
    Failure = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5
}

// sealed = nobody can inherit from Error
// This keeps it simple and predictable
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