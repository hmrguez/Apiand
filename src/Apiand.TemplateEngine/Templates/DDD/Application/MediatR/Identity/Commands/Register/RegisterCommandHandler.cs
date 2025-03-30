using Apiand.Extensions.Models;
using XXXnameXXX.Domain.Entities.Identity;
using MediatR;
using XXXnameXXX.Application.Identity.Commands.Login;
using XXXnameXXX.Application.Identity.Dtos;
using XXXnameXXX.Application.Identity.Services;

namespace XXXnameXXX.Application.Identity.Commands.Register;

public class RegisterCommandHandler(IUserService userService, IMediator mediator): IRequestHandler<RegisterCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await userService.FindByEmail(request.Email);
        if (user != null)
        {
            return UserErrors.EmailAlreadyExists;
        }

        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = request.Password
        };
        
        await userService.AddAsync(newUser);

        var loginRequest = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        return await mediator.Send(loginRequest, cancellationToken);
    }
}