namespace MediSync.Web.Services.ServiceResponse;

/// <summary>
/// Represents the result of a service call.
/// Contains either a successful value or error information.
/// </summary>
/// <typeparam name="T">The type of the successful response value.</typeparam>
public sealed class ServiceResponseMessage<T>
{
    public bool IsSuccess { get; init; }

    public T? Value { get; init; }

    public Error Error { get; init; } = Error.None;

    /// <summary>
    /// Creates a successful service response.
    /// </summary>
    /// <param name="value">The value returned by the service.</param>
    /// <returns>A successful <see cref="ServiceResponseMessage{T}"/>.</returns>
    public static ServiceResponseMessage<T> Success(T value) => new() { IsSuccess = true, Value = value, Error = Error.None };


    /// <summary>
    /// Creates a failed service response.
    /// </summary>
    /// <param name="code">A unique error code.</param>
    /// <param name="message">A description of the error.</param>
    /// <param name="type">The category of the error.</param>
    /// <returns>A failed <see cref="ServiceResponseMessage{T}"/>.</returns>
    public static ServiceResponseMessage<T> Failure(string code, string message, string type) => new() { IsSuccess = false, Value = default, Error = new Error(code, message, type) };
}

/// <summary>
/// Represents error details returned from a service.
/// </summary>
/// <param name="Code">A unique error identifier.</param>
/// <param name="Message">A human-readable error description.</param>
/// <param name="Type">The category of the error.</param>
public record Error(string Code, string Message, string Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, string.Empty);
}