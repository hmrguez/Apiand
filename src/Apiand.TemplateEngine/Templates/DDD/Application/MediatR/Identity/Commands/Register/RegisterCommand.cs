using Apiand.Extensions.Models;
using MediatR;
using XXXnameXXX.Application.Identity.Dtos;

namespace XXXnameXXX.Application.Identity.Commands.Register;

public record RegisterCommand: IRequest<Result<LoginResponse>>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}