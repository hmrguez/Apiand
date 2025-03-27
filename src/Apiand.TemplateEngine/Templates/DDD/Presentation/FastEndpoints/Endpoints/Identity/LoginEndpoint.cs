using XXXnameXXX.Application.Identity.Commands.Login;
using XXXnameXXX.Application.Identity.Dtos;
using MediatR;
using XXXnameXXX.Presentation.Models;

namespace XXXnameXXX.Presentation.Endpoints.Identity;

public class LoginEndpoint(IMediator mediator) : CustomEndpoint<LoginCommand, LoginResponse>(mediator)
{
    protected override string Route => "login";
    protected override Models.HttpMethod Method => Models.HttpMethod.Post;
    protected override bool Secure => false;
}