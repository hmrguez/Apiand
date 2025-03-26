using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;

namespace Apiand.TemplateEngine.Architectures.DDD.Commands;

public class GenerateEndpoint : IGenerateEndpoint
{
    public string ArchName { get; set; } = DddUtils.Name;

    public void Handle(string workingDirectory, string projectDirectory, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var normalizedName = argument;

        // Parse the endpoint name for subdirectories (e.g., "Identity.User" -> ["Identity", "User"])
        string[] nameParts = normalizedName.Split('.');
        string endpointClassName = nameParts[^1]; // Last part is the actual endpoint name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));

        // Get HTTP method from extra data or default to POST (which implies command)
        string httpMethodStr = extraData.ContainsKey("http-method") ? extraData["http-method"] : "Get";
        bool isQuery = httpMethodStr.Equals("Get", StringComparison.OrdinalIgnoreCase);

        // Determine request type name based on whether it's a command or query
        string requestTypeName = isQuery ? $"{endpointClassName}Query" : $"{endpointClassName}Command";

        // Find the appropriate projects for API and Application layers
        string apiProject = null;
        string applicationProject = null;

        // For DDD architecture, find API/Presentation and Application projects
        var projectFiles = Directory.GetFiles(projectDirectory, "*.csproj", SearchOption.AllDirectories);

        foreach (var projectFile in projectFiles)
        {
            string projectFileName = Path.GetFileNameWithoutExtension(projectFile);
            if (projectFileName.EndsWith("Api") || projectFileName.EndsWith("Presentation"))
            {
                apiProject = Path.GetDirectoryName(projectFile);
            }
            else if (projectFileName.EndsWith("Application"))
            {
                applicationProject = Path.GetDirectoryName(projectFile);
            }
        }

        if (apiProject == null || applicationProject == null)
        {
            messenger.WriteErrorMessage(
                "Could not find required API/Presentation and Application projects in DDD architecture.");
            return;
        }

        // Create directories
        string endpointsDir = Path.Combine(apiProject, "Endpoints", subDirPath);
        string requestsDir = Path.Combine(applicationProject, isQuery ? "Queries" : "Commands", subDirPath,
            endpointClassName);
        string dtosDir = Path.Combine(applicationProject, "Dtos", subDirPath);

        Directory.CreateDirectory(endpointsDir);
        Directory.CreateDirectory(requestsDir);
        Directory.CreateDirectory(dtosDir);

        // Create the request class (command or query)
        string requestContent =
            $$"""
              using MediatR;
              using Apiand.Extensions.Models;
              using {{configuration.ProjectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

              namespace {{configuration.ProjectName}}.Application.{{(isQuery ? "Queries" : "Commands")}}{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}}.{{endpointClassName}};

              public class {{requestTypeName}}: IRequest<Result<{{endpointClassName}}Response>>
              {
                  // TODO: Add {{(isQuery ? "query" : "command")}} properties
              }
              """;

        // Create the handler class
        string handlerContent =
            $$"""
              using Apiand.Extensions.Models;
              using MediatR;
              using Microsoft.Extensions.Logging;
              using {{configuration.ProjectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

              namespace {{configuration.ProjectName}}.Application.{{(isQuery ? "Queries" : "Commands")}}{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}}.{{endpointClassName}};

              public class {{requestTypeName}}Handler(ILogger<{{requestTypeName}}Handler> logger) : IRequestHandler<{{requestTypeName}}, Result<{{endpointClassName}}Response>>
              {
                  public async Task<Result<{{endpointClassName}}Response>> Handle({{requestTypeName}} request, CancellationToken cancellationToken)
                  {
                      logger.LogInformation("Processing {{requestTypeName}}");
                      
                      // TODO: Implement handler logic
                      
                      return new {{endpointClassName}}Response();
                  }
              }
              """;

        // Create the response DTO
        string responseContent =
            $$"""
              namespace {{configuration.ProjectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

              public class {{endpointClassName}}Response
              {
                  // TODO: Add response properties
              }
              """;

        // Create the endpoint class
        string endpointContent =
            $$"""
              using {{configuration.ProjectName}}.Application.{{(isQuery ? "Queries" : "Commands")}}{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}}.{{endpointClassName}};
              using {{configuration.ProjectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
              using MediatR;
              using {{configuration.ProjectName}}.Api.Models;

              namespace {{configuration.ProjectName}}.Api.Endpoints{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

              public class {{endpointClassName}}Endpoint(IMediator mediator) : CustomEndpoint<{{requestTypeName}}, {{endpointClassName}}Response>(mediator)
              {
                  protected override string Route => "{{endpointClassName.ToLowerInvariant()}}";
                  protected override Models.HttpMethod Method => Models.HttpMethod.{{httpMethodStr}};
                  protected override bool Secure => {{(isQuery ? "false" : "true")}};
              }
              """;

        // Write the files
        string requestPath = Path.Combine(requestsDir, $"{requestTypeName}.cs");
        string handlerPath = Path.Combine(requestsDir, $"{requestTypeName}Handler.cs");
        string responsePath = Path.Combine(dtosDir, $"{endpointClassName}Response.cs");
        string endpointPath = Path.Combine(endpointsDir, $"{endpointClassName}Endpoint.cs");

        File.WriteAllText(requestPath, requestContent);
        File.WriteAllText(handlerPath, handlerContent);
        File.WriteAllText(responsePath, responseContent);
        File.WriteAllText(endpointPath, endpointContent);

        messenger.WriteStatusMessage(
            $"Created {(isQuery ? "query" : "command")} at {Path.GetRelativePath(projectDirectory, requestPath)}");
        messenger.WriteStatusMessage($"Created handler at {Path.GetRelativePath(projectDirectory, handlerPath)}");
        messenger.WriteStatusMessage($"Created response DTO at {Path.GetRelativePath(projectDirectory, responsePath)}");
        messenger.WriteStatusMessage($"Created endpoint at {Path.GetRelativePath(projectDirectory, endpointPath)}");
    }
}