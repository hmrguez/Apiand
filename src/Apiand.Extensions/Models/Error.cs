using System.Net;

namespace Apiand.Extensions.Models;

public sealed record Error(string Code, string Description, HttpStatusCode StatusCode = HttpStatusCode.BadRequest);