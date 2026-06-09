namespace MediSync.BuildingBlocks.Common;

/// <summary>
/// T = type of success value
/// Example: Result<Guid> means success returns a Guid (the new user's Id)
/// Example: Result<UserDto> means success returns a UserDto
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T> 
{
    // The success value — only has meaning when IsSuccess is true
    // Nullable because on failure there is no value
    public T? Value { get; }

    public Error Error { get; }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    private Result(T value)
    {
        Value = value;
        Error = Error.None;  // no error on success
        IsSuccess = true;
    }

    private Result(Error error)
    {
        Value = default;  // null/default on failure
        Error = error;
        IsSuccess = false;
    }

    // Static factory methods — the only way to create a Result
    // Usage: Result<Guid>.Success(user.Id)
    // Usage: Result<Guid>.Failure(UserErrors.EmailAlreadyExists(email))
    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);

}
