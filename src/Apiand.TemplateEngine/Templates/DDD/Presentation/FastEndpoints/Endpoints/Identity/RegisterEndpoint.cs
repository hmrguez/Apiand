using XXXnameXXX.Application.Identity.Commands.Register;
using XXXnameXXX.Application.Identity.Dtos;
using MediatR;
using XXXnameXXX.Api.Models;
using HttpMethod = XXXnameXXX.Api.Models.HttpMethod;
using Models_HttpMethod = XXXnameXXX.Api.Models.HttpMethod;

namespace XXXnameXXX.Api.Endpoints.Identity;

public class RegisterEndpoint(IMediator mediator) : CustomEndpoint<RegisterCommand, LoginResponse>(mediator)
{
    protected override string Route => "register";
    protected override Models_HttpMethod Method => Models.HttpMethod.Post;
    protected override bool Secure => false;
}