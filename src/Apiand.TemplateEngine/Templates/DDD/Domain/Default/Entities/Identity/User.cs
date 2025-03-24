using Apiand.Extensions.DDD;

namespace XXXnameXXX.Domain.Entities.Identity;

public class User: Entity
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}