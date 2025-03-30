using XXXnameXXX.Domain.Entities.Todo;
using MediatR;
using XXXnameXXX.Application.Contracts;

namespace XXXnameXXX.Application.Todos.Commands.Delete;

public class DeleteCommandHandler(IRepository<Todo> repository) : IRequestHandler<DeleteCommand>
{
    public async Task Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteAsync(request.Id);
    }
}