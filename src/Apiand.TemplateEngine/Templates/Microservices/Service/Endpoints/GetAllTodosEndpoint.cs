using FastEndpoints;
using XXXnameXXX.XXXserviceXXX.Models;

namespace XXXnameXXX.XXXserviceXXX.Endpoints;

public class TodoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetAllTodosEndpoint : EndpointWithoutRequest<List<TodoResponse>>
{
    // In-memory storage for todos
    private static readonly List<Todo> _todos = new()
    {
        new Todo { Title = "Learn FastEndpoints", Description = "Study the FastEndpoints documentation" },
        new Todo { Title = "Build API", Description = "Create a sample API using FastEndpoints" },
        new Todo { Title = "Test API", Description = "Test the API endpoints" }
    };

    public override void Configure()
    {
        Get("/todos");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Gets all todo items";
            s.Description = "Retrieves a list of all todo items";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = _todos.Select(t => new TodoResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            CreatedAt = t.CreatedAt
        }).ToList();

        await SendOkAsync(response, ct);
    }
}