using XXXnameXXX.Domain.Entities.Todo;
using MediatR;
using XXXnameXXX.Application.Contracts;

namespace XXXnameXXX.Application.Todos.Commands.Update;

public class UpdateCommandHandler(IRepository<Todo> repository):IRequestHandler<UpdateCommand>
{
    public async Task Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo(request.Title, request.DueBy);
        await repository.UpdateAsync(request.Id, todo);
    }
}