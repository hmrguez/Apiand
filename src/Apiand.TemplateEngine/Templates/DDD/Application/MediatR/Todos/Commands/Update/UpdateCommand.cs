using MediatR;

namespace XXXnameXXX.Application.Todos.Commands.Update;

public class UpdateCommand : IRequest
{
    public string Id { get; set; }
    public string Title { get; set; }
    public DateOnly? DueBy { get; set; }
}