using Apiand.Extensions.Models;
using MediatR;
using XXXnameXXX.Application.Identity.Dtos;

namespace XXXnameXXX.Application.Identity.Commands.Login;

public record LoginCommand: IRequest<Result<LoginResponse>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}