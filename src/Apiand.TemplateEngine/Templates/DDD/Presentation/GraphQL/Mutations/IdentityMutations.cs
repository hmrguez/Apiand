using MediatR;
using XXXnameXXX.Application.Identity.Commands.Login;
using XXXnameXXX.Application.Identity.Commands.Register;
using XXXnameXXX.Application.Identity.Dtos;

namespace XXXnameXXX.Presentation.Mutations;

public partial class Mutation
{
    public async Task<Apiand.Extensions.Models.Result<LoginResponse>> Login([Service] IMediator mediator, string email, string password)
    {
        return await mediator.Send(new LoginCommand()
        {
            Email = email,
            Password = password
        });
    }

    public async Task<Apiand.Extensions.Models.Result<LoginResponse>> Register([Service] IMediator mediator, string email, string password)
    {
        return await mediator.Send(new RegisterCommand()
        {
            Email = email,
            Password = password
        });
    }
}