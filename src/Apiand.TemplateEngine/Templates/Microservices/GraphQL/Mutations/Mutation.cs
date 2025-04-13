using XXXnameXXX.XXXserviceXXX.Models;
using XXXnameXXX.XXXserviceXXX.Services;

namespace XXXnameXXX.XXXserviceXXX.Mutations;

public class Mutation
{
    public Todo AddTodo([Service] TodoRepository repository, string title)
    {
        var todo = new Todo
        {
            Title = title,
            IsCompleted = false
        };

        return repository.Add(todo);
    }
}