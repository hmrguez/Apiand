using Apiand.Extensions.Models;
using XXXnameXXX.Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Logging;
using XXXnameXXX.Application.Identity.Dtos;
using XXXnameXXX.Application.Identity.Services;

namespace XXXnameXXX.Application.Identity.Commands.Login;

public class LoginCommandHandler(IUserService userService, ILogger<LoginCommandHandler> logger): IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userService.FindByEmail(request.Email);
        if (user == null || !userService.VerifyPassword(user, request.Password))
        {
            logger.LogWarning("Invalid email or password");
            return UserErrors.InvalidCredentials;
        }

        var token = userService.IssueToken(user);
        return new LoginResponse(token);
    }
}