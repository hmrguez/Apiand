using Apiand.Extensions.Models;
using XXXnameXXX.Domain.Entities.Todo;
using MediatR;

namespace XXXnameXXX.Application.Todos.Commands.Add;

public class AddCommand : IRequest
{
    public string Title { get; set; }
    public DateOnly? DueBy { get; set; }
}