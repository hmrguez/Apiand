using XXXnameXXX.XXXserviceXXX.Models;

namespace XXXnameXXX.XXXserviceXXX.Services;

public class TodoRepository
{
    private readonly List<Todo> _todos = new();

    public IQueryable<Todo> GetAll() => _todos.AsQueryable();

    public Todo Add(Todo todo)
    {
        todo.Id = Guid.NewGuid();
        _todos.Add(todo);
        return todo;
    }
}