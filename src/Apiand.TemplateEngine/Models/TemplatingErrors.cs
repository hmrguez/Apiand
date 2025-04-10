using Apiand.Extensions.Models;

namespace Apiand.TemplateEngine.Models;

public static class TemplatingErrors
{
    public static readonly Error DddProjectNotFound = new("DDD.001", "Required projects could not be found in DDD architecture.");
    public static readonly Error ApiApplicationProjectsNotFound = new("DDD.002", "Could not find required API/Presentation and Application projects in DDD architecture.");
    public static readonly Error DomainProjectNotFound = new("DDD.003", "Could not find Domain project in DDD architecture.");
    public static readonly Error ApplicationInfrastructureProjectsNotFound = new("DDD.004", "Could not find required Application and Infrastructure projects in DDD architecture.");
    public static readonly Error ProjectNotFound = new("404", "Could not find a main project in folder");
    public static readonly Error SolutionNotFound = new("404", "Could not find a solution in folder");
}
