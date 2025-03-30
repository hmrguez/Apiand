using XXXnameXXX.Domain.Entities.Todo;
using MediatR;
using XXXnameXXX.Application.Contracts;

namespace XXXnameXXX.Application.Todos.Commands.Add;

public class AddCommandHandler(IRepository<Todo> repository) : IRequestHandler<AddCommand>
{

    public async Task Handle(AddCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo(request.Title, request.DueBy);
        await repository.AddAsync(todo);
    }
}