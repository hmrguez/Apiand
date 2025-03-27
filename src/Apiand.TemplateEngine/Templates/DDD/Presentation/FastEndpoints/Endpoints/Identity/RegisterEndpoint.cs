using XXXnameXXX.Application.Identity.Commands.Register;
using XXXnameXXX.Application.Identity.Dtos;
using MediatR;
using XXXnameXXX.Presentation.Models;

namespace XXXnameXXX.Presentation.Endpoints.Identity;

public class RegisterEndpoint(IMediator mediator) : CustomEndpoint<RegisterCommand, LoginResponse>(mediator)
{
    protected override string Route => "register";
    protected override Models.HttpMethod Method => Models.HttpMethod.Post;
    protected override bool Secure => false;
}