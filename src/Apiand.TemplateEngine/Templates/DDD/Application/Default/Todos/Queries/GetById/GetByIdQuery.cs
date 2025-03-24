using Apiand.Extensions.Models;
using MediatR;
using XXXnameXXX.Application.Todos.Dtos;

namespace XXXnameXXX.Application.Todos.Queries.GetById;

public class GetByIdQuery:IRequest<Result<TodoDto>>
{
    public string Id { get; set; }
}