using Apiand.Extensions.Models;
using XXXnameXXX.Application.Todos.Dtos;
using XXXnameXXX.Application.Todos.Queries.GetAll;
using MediatR;
using XXXnameXXX.Presentation.Models;

namespace XXXnameXXX.Presentation.Endpoints.Todos;

public class GetAll(IMediator mediator) : CustomEndpoint<GetAllTodoQuery, List<TodoDto>>(mediator)
{
    protected override string Route => "/todos";
    protected override Models.HttpMethod Method => Models.HttpMethod.Get;
    protected override bool Secure => true;
    
    public override void Configure()
    {
        base.Configure();
        
        Description(x => x.WithTags("Todos"));
    }
}