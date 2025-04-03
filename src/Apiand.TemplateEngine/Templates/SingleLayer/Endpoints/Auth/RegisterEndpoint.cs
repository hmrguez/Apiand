using FastEndpoints;
using XXXnameXXX.Dtos;
using XXXnameXXX.Services;

namespace XXXnameXXX.Endpoints.Auth;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? UserId { get; set; }
}

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly IUserService _userService;
    
    public RegisterEndpoint(IUserService userService)
    {
        _userService = userService;
    }
    
    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Registers a new user";
            s.Description = "Creates a new user account in the system";
        });
        
        Description(x => x.WithTags("Auth"));
    }
    
    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var result = await _userService.RegisterAsync(req);
        
        await SendAsync(new RegisterResponse
        {
            Success = result.Success,
            Message = result.Success ? "Registration successful" : result.ErrorMessage ?? "Registration failed",
            UserId = result.UserId
        }, cancellation: ct);
    }
}
