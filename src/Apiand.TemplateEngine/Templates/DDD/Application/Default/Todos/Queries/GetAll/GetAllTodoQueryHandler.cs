using Apiand.Extensions.Models;
using XXXnameXXX.Domain.Entities.Todo;
using Mapster;
using MediatR;
using XXXnameXXX.Application.Contracts;
using XXXnameXXX.Application.Todos.Dtos;

namespace XXXnameXXX.Application.Todos.Queries.GetAll;

public class GetAllTodoQueryHandler(IRepository<Todo> repository): IRequestHandler<GetAllTodoQuery, Result<List<TodoDto>>>
{

    public async Task<Result<List<TodoDto>>> Handle(GetAllTodoQuery request, CancellationToken cancellationToken)
    {
        var todos = await repository.GetAllAsync();
        var todoDtos = todos.Select(todo => todo.Adapt<TodoDto>()).ToList();
        return todoDtos;
    }
}