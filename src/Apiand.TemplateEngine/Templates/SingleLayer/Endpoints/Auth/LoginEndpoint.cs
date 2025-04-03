using FastEndpoints;
using XXXnameXXX.Services;
using XXXnameXXX.Dtos;

namespace XXXnameXXX.Endpoints.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
}

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IUserService _userService;
    
    public LoginEndpoint(IUserService userService)
    {
        _userService = userService;
    }
    
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Authenticates a user";
            s.Description = "Validates user credentials and returns user information if valid";
        });
        Description(x => x.WithTags("Auth"));
    }
    
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userService.AuthenticateAsync(req.Email, req.Password);
        
        if (user == null)
        {
            await SendAsync(new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            }, cancellation: ct);
            return;
        }
        
        await SendAsync(new LoginResponse
        {
            Success = true,
            Message = "Authentication successful",
            User = user
        }, cancellation: ct);
    }
}
