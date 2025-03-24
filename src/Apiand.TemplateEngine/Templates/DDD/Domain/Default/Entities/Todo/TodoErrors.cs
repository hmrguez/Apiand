using System.Net;
using Apiand.Extensions.Models;

namespace XXXnameXXX.Domain.Entities.Todo;

public class TodoErrors
{ 
    public static Error TodoIdNotFound => new("todo_id_not_found", "Todo id not found", HttpStatusCode.NotFound);
}