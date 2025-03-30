using XXXnameXXX.Domain.Entities.Identity;

namespace XXXnameXXX.Application.Identity.Services;

public interface IUserService
{
    public Task AddAsync(User user);
    public Task<User?> FindByUsername(string username);
    public Task<User?> FindByEmail(string email);
    public bool VerifyPassword(User user, string password);
    public string IssueToken(User user);
}