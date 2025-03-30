using XXXnameXXX.Domain.Entities.Todo;
using Mapster;
using XXXnameXXX.Application.Todos.Dtos;

namespace XXXnameXXX.Application.Todos;

public class TodoMapping: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Todo, TodoDto>();
    }
}