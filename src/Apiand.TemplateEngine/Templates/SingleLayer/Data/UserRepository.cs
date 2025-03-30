using Dapper;
using XXXnameXXX.Models;

namespace XXXnameXXX.Data;

public class UserRepository : DapperRepository<User>, IUserRepository
{
    public UserRepository(IConfiguration configuration) 
        : base(configuration, "Users")
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM \"Users\" WHERE \"Username\" = @Username", 
            new { Username = username });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM \"Users\" WHERE \"Email\" = @Email", 
            new { Email = email });
    }
}
