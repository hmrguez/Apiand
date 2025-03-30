using XXXnameXXX.Models;
using XXXnameXXX.Dtos;

namespace XXXnameXXX.Services;

public interface IUserService
{
    Task<UserDto?> AuthenticateAsync(string username, string password);
    Task<RegistrationResult> RegisterAsync(RegisterRequest model);
    Task<User?> GetByIdAsync(int id);
}

public class RegistrationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? UserId { get; set; }
}
