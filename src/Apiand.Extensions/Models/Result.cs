namespace Apiand.Extensions.Models;

/// <summary>
/// Represents the result of an operation without a specific return value.
/// </summary>
/// <remarks>
/// This class provides a standardized way to handle operation results with potential errors,
/// making it easier to manage success/failure outcomes throughout the application.
/// </remarks>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    /// <value>
    /// <c>true</c> if the operation succeeded (no error); otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccess => Error == null;

    /// <summary>
    /// Gets the error information if the operation failed.
    /// </summary>
    /// <value>
    /// An <see cref="Error"/> object if the operation failed; otherwise, <c>null</c>.
    /// </value>
    public Error? Error { get; protected init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/> with no error.</returns>
    public static Result Succeed()
    {
        return new Result
        {
            Error = null
        };
    }

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the operation to fail.</param>
    /// <returns>A failed <see cref="Result"/> containing the error information.</returns>
    public static Result Fail(Error error)
    {
        return new Result
        {
            Error = error
        };
    }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <returns>A failed <see cref="Result"/> containing the specified error.</returns>
    public static implicit operator Result(Error error) => Fail(error);
}

/// <summary>
/// Represents the result of an operation that returns a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
/// <remarks>
/// This generic version extends <see cref="Result"/> to include a data payload when the operation succeeds.
/// It provides a consistent pattern for returning both success values and failure errors.
/// </remarks>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the data returned by the operation if it was successful.
    /// </summary>
    /// <value>
    /// The operation's result data if successful; otherwise, the default value of <typeparamref name="T"/>.
    /// </value>
    public T? Data { get; private init; }

    /// <summary>
    /// Creates a successful result with the specified data.
    /// </summary>
    /// <param name="data">The data to include in the successful result.</param>
    /// <returns>A successful <see cref="Result{T}"/> containing the specified data.</returns>
    public static Result<T> Succeed(T data)
    {
        return new Result<T>
        {
            Data = data,
            Error = null
        };
    }

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the operation to fail.</param>
    /// <returns>A failed <see cref="Result{T}"/> containing the error information.</returns>
    public static new Result<T> Fail(Error error)
    {
        return new Result<T>
        {
            Error = error
        };
    }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <returns>A failed <see cref="Result{T}"/> containing the specified error.</returns>
    public static implicit operator Result<T>(Error error) => Fail(error);

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <returns>A successful <see cref="Result{T}"/> containing the specified data.</returns>
    public static implicit operator Result<T>(T data) => Succeed(data);
}