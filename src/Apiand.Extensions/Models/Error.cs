using System.Net;

namespace Apiand.Extensions.Models;

/// <summary>
/// Represents an API error with a code, description, and HTTP status code.
/// </summary>
/// <remarks>
/// This record is designed to standardize error responses across an API,
/// making it easier to provide consistent error information to clients.
/// </remarks>
/// <param name="Code">A unique identifier or code representing the specific error.</param>
/// <param name="Description">A human-readable description of the error.</param>
/// <param name="StatusCode">The HTTP status code to return with this error. Defaults to 400 (Bad Request).</param>
public sealed record Error(string Code, string Description, HttpStatusCode StatusCode = HttpStatusCode.BadRequest);