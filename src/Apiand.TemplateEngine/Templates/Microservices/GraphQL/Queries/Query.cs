using XXXnameXXX.XXXserviceXXX.Models;
using XXXnameXXX.XXXserviceXXX.Services;

namespace XXXnameXXX.XXXserviceXXX.Queries;

public class Query
{
    public IQueryable<Todo> GetTodos([Service] TodoRepository repository)
    {
        return repository.GetAll();
    }
}