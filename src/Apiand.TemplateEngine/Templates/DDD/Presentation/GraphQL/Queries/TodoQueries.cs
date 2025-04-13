using HotChocolate.Authorization;
using MediatR;
using XXXnameXXX.Application.Todos.Dtos;
using XXXnameXXX.Application.Todos.Queries.GetAll;

namespace XXXnameXXX.Presentation.Queries;

public partial class Query
{
    [Authorize]
    public async Task<Apiand.Extensions.Models.Result<List<TodoDto>>> GetTodos([Service] IMediator mediator)
    {
        return await mediator.Send(new GetAllTodoQuery());
    }
}