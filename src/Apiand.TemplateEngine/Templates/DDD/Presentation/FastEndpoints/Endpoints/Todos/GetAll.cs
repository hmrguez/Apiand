using XXXnameXXX.Application.Todos.Dtos;
using XXXnameXXX.Application.Todos.Queries.GetAll;
using MediatR;
using XXXnameXXX.Api.Models;
using HttpMethod = XXXnameXXX.Api.Models.HttpMethod;
using Models_HttpMethod = XXXnameXXX.Api.Models.HttpMethod;

namespace XXXnameXXX.Api.Endpoints.Todos;

public class GetAll(IMediator mediator) : CustomEndpoint<GetAllTodoQuery, List<TodoDto>>(mediator)
{
    protected override string Route => "/todos";
    protected override Models_HttpMethod Method => Models.HttpMethod.Get;
    protected override bool Secure => true;
}