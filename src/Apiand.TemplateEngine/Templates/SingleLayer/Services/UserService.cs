using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using XXXnameXXX.Config;
using XXXnameXXX.Data;
using XXXnameXXX.Dtos;
using XXXnameXXX.Models;
using Apiand.Extensions.Service;

namespace XXXnameXXX.Services;


[Service]
public class UserService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    : IUserService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public async Task<UserDto?> AuthenticateAsync(string email, string password)
    {
        var user = await userRepository.GetByEmailAsync(email);

        // Return null if user not found or password doesn't match
        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) ==
            PasswordVerificationResult.Failed)
            return null;


        // Generate JWT token
        var token = GenerateJwtToken(user);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = token
        };
    }

    public async Task<RegistrationResult> RegisterAsync(RegisterRequest model)
    {
        // Check if username already exists
        if (await userRepository.GetByUsernameAsync(model.Username) != null)
        {
            return new RegistrationResult
            {
                Success = false,
                ErrorMessage = "Username already exists"
            };
        }

        // Check if email already exists
        if (await userRepository.GetByEmailAsync(model.Email) != null)
        {
            return new RegistrationResult
            {
                Success = false,
                ErrorMessage = "Email already exists"
            };
        }

        // Create new user
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow
        };

        // Hash the password
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        // Save user to database
        var userId = await userRepository.AddAsync(user);

        return new RegistrationResult
        {
            Success = true,
            UserId = userId
        };
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await userRepository.GetByIdAsync(id);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpirationDays),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}