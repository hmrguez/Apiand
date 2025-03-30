using Apiand.Extensions.Models;
using MediatR;
using XXXnameXXX.Application.Todos.Dtos;

namespace XXXnameXXX.Application.Todos.Queries.GetAll;

public class GetAllTodoQuery: IRequest<Result<List<TodoDto>>>
{
    public string Email { get; set; }
}