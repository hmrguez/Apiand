using System.Net;
using Apiand.Extensions.Models;

namespace XXXnameXXX.Domain.Entities.Identity;

public static class UserErrors
{
    public static Error EmailAlreadyExists => new("email_already_exists", "Email already exists");
    public static Error UserNotFound => new("user_not_found", "User not found", HttpStatusCode.NotFound);
    public static Error InvalidCredentials => new("invalid_credentials", "Invalid credentials");
}