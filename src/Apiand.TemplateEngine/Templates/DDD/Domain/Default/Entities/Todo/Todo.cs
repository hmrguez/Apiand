using Apiand.Extensions.DDD;

namespace XXXnameXXX.Domain.Entities.Todo;

public class Todo : Entity
{

    public Todo(string title)
    {
        Title = title;
    }
    
    public Todo(string title, DateOnly? dueBy = null)
    {
        Title = title;
        DueBy = dueBy;
    }

    public string Title { get; init; }
    public DateOnly? DueBy { get; init; }
    public bool IsComplete { get; init; } = false;
}