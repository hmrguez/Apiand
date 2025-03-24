namespace XXXnameXXX.Application.Todos.Dtos;

public class TodoDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public DateOnly? DueDate { get; set; }
    public bool IsComplete { get; set; }
}