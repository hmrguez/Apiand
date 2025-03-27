using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Apiand.Extensions.Service;
using XXXnameXXX.Application.Contracts;
using XXXnameXXX.Application.Identity.Dtos;
using XXXnameXXX.Application.Identity.Services;
using XXXnameXXX.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace XXXnameXXX.Infrastructure.Identity;

[Service]
public class UserService(IRepository<User> repository, IPasswordHasher<User> passwordHasher, IConfiguration configuration): IUserService
{
    public Task AddAsync(User user)
    {
        user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);
        return repository.AddAsync(user);
    }

    public async Task<User?> FindByUsername(string username)
    {
        return (await repository.FindAsync(user => user.Username == username)).FirstOrDefault();
    }

    public async Task<User?> FindByEmail(string email)
    {
        return (await repository.FindAsync(user => user.Email == email)).FirstOrDefault();
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }

    public string IssueToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("id", user.Id)
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}