using Apiand.Extensions.Models;
using XXXnameXXX.Domain.Entities.Todo;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using XXXnameXXX.Application.Contracts;
using XXXnameXXX.Application.Todos.Dtos;

namespace XXXnameXXX.Application.Todos.Queries.GetById;

public class GetByIdQueryHandler(IRepository<Todo> repository, ILogger<GetByIdQueryHandler> logger) : IRequestHandler<GetByIdQuery, Result<TodoDto>>
{
    public async Task<Result<TodoDto>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await repository.GetByIdAsync(request.Id);

        if (todo is null)
        {
            logger.LogWarning($"Id: {request.Id} not found");
            return TodoErrors.TodoIdNotFound;
        }
        
        var todoDto = todo.Adapt<TodoDto>();
        return todoDto;
    }
}