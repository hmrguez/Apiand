using Dapper.Contrib.Extensions;

namespace XXXnameXXX.Models;

[Table("Users")]
public class User
{
    [System.ComponentModel.DataAnnotations.Key]
    [ExplicitKey]
    public string Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    [Write(false)]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
