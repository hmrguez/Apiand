using MediatR;

namespace XXXnameXXX.Application.Todos.Commands.Delete;

public class DeleteCommand : IRequest
{
    public string Id { get; set; }
}