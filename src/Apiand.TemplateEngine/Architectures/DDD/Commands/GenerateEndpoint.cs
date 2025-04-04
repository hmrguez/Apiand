using Apiand.Extensions.Models;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Architectures.DDD.Commands;

public class GenerateEndpoint : IGenerateEndpoint
{
    public string ArchName { get; set; } = DddUtils.Name;

    public Result Handle(string workingDirectory, string projectDirectory, string argument,
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
            return Result.Fail(TemplatingErrors.ApiApplicationProjectsNotFound);
        }

        // Create directories
        string endpointsDir = Path.Combine(apiProject, "Endpoints", subDirPath);
        string requestsDir = Path.Combine(applicationProject, isQuery ? "Queries" : "Commands", subDirPath,
            endpointClassName);
        string dtosDir = Path.Combine(applicationProject, "Dtos", subDirPath);

        Directory.CreateDirectory(endpointsDir);
        Directory.CreateDirectory(requestsDir);
        Directory.CreateDirectory(dtosDir);

        // Generate file contents using CodeBlocks
        string requestContent = CodeBlocks.GenerateEndpointRequest(
            configuration.ProjectName, endpointClassName, requestTypeName, subDirPath, isQuery);

        string handlerContent = CodeBlocks.GenerateEndpointHandler(
            configuration.ProjectName, endpointClassName, requestTypeName, subDirPath, isQuery);

        string responseContent = CodeBlocks.GenerateEndpointResponse(
            configuration.ProjectName, endpointClassName, subDirPath);

        string endpointContent = CodeBlocks.GenerateEndpointClass(
            configuration.ProjectName, endpointClassName, requestTypeName, subDirPath, httpMethodStr, isQuery);

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

        return Result.Succeed();
    }
}
