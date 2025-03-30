using XXXnameXXX.Models;

namespace XXXnameXXX.Data;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
}
