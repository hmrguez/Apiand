using Apiand.Extensions.Models;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using Apiand.TemplateEngine.Utils;
using System.Text;

namespace Apiand.TemplateEngine.Architectures.DDD.Commands;

public class GenerateEndpoint : IGenerateEndpoint
{
    public string ArchName { get; set; } = DddUtils.Name;

    public Result Handle(string workingDirectory, string projectDirectory, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var dddConfig = configuration as DddTemplateConfiguration;
        if (dddConfig is null)
            return TemplatingErrors.InvalidProjectConfiguration;
        
        var normalizedName = argument;

        // Parse the endpoint name for subdirectories (e.g., "Identity.User" -> ["Identity", "User"])
        string[] nameParts = normalizedName.Split('.');
        string endpointClassName = nameParts[^1]; // Last part is the actual endpoint name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));

        // Get HTTP method from extra data or default to GET (for Query)
        string methodStr = extraData.ContainsKey("method") ? extraData["method"] : "Get";
        bool isQuery;

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

        // Handle based on presentation type
        if (dddConfig.Presentation is Option.FastEndpoints)
        {
            // For FastEndpoints
            isQuery = methodStr.Equals("Get", StringComparison.OrdinalIgnoreCase);
            return HandleFastEndpoints(apiProject, applicationProject, projectDirectory, endpointClassName, subDirPath, 
                methodStr, isQuery, configuration.ProjectName, messenger);
        }
        else
        {
            // For GraphQL
            isQuery = methodStr.Equals("Query", StringComparison.OrdinalIgnoreCase);
            return HandleGraphQL(apiProject, applicationProject, projectDirectory, endpointClassName, nameParts[0], 
                subDirPath, isQuery, configuration.ProjectName, messenger);
        }
    }

    private Result HandleFastEndpoints(string apiProject, string applicationProject, string projectDirectory, 
        string endpointClassName, string subDirPath, string httpMethodStr, bool isQuery, string projectName, 
        IMessenger messenger)
    {
        // Determine request type name based on whether it's a command or query
        string requestTypeName = isQuery ? $"{endpointClassName}Query" : $"{endpointClassName}Command";

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
            projectName, endpointClassName, requestTypeName, subDirPath, isQuery);

        string handlerContent = CodeBlocks.GenerateEndpointHandler(
            projectName, endpointClassName, requestTypeName, subDirPath, isQuery);

        string responseContent = CodeBlocks.GenerateEndpointResponse(
            projectName, endpointClassName, subDirPath);

        string endpointContent = CodeBlocks.GenerateEndpointClass(
            projectName, endpointClassName, requestTypeName, subDirPath, httpMethodStr, isQuery);

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

    private Result HandleGraphQL(string apiProject, string applicationProject, string projectDirectory, 
        string operationName, string domainName, string subDirPath, bool isQuery, string projectName, 
        IMessenger messenger)
    {
        // Determine request type name and directories based on operation type
        string requestTypeName = isQuery ? $"{operationName}Query" : $"{operationName}Command";
        
        string requestsDir = Path.Combine(applicationProject, isQuery ? "Queries" : "Commands", subDirPath,
            operationName);
        string dtosDir = Path.Combine(applicationProject, "Dtos", subDirPath);
        string graphqlDir = Path.Combine(apiProject, isQuery ? "Queries" : "Mutations", subDirPath);

        Directory.CreateDirectory(requestsDir);
        Directory.CreateDirectory(dtosDir);
        Directory.CreateDirectory(graphqlDir);

        // Generate request handler and response
        string requestContent = CodeBlocks.GenerateEndpointRequest(
            projectName, operationName, requestTypeName, subDirPath, isQuery);

        string handlerContent = CodeBlocks.GenerateEndpointHandler(
            projectName, operationName, requestTypeName, subDirPath, isQuery);

        string responseContent = CodeBlocks.GenerateEndpointResponse(
            projectName, operationName, subDirPath);

        // Write the request files
        string requestPath = Path.Combine(requestsDir, $"{requestTypeName}.cs");
        string handlerPath = Path.Combine(requestsDir, $"{requestTypeName}Handler.cs");
        string responsePath = Path.Combine(dtosDir, $"{operationName}Response.cs");

        File.WriteAllText(requestPath, requestContent);
        File.WriteAllText(handlerPath, handlerContent);
        File.WriteAllText(responsePath, responseContent);

        // Handle GraphQL file
        string graphqlFileName = $"{domainName}{(isQuery ? "Queries" : "Mutations")}.cs";
        string graphqlFilePath = Path.Combine(graphqlDir, graphqlFileName);
        bool isNewFile = !File.Exists(graphqlFilePath);

        string graphqlContent;
        if (isQuery)
        {
            graphqlContent = CodeBlocks.GenerateGraphQLQuery(projectName, domainName, operationName, subDirPath, isNewFile);
        }
        else
        {
            graphqlContent = CodeBlocks.GenerateGraphQLMutation(projectName, domainName, operationName, subDirPath, isNewFile);
        }

        if (isNewFile)
        {
            // Create a new file
            File.WriteAllText(graphqlFilePath, graphqlContent);
        }
        else
        {
            // Append to existing file - read file, insert before last closing brace
            string existingContent = File.ReadAllText(graphqlFilePath);
            int insertPosition = existingContent.LastIndexOf('}');
            if (insertPosition > 0)
            {
                StringBuilder updatedContent = new StringBuilder(existingContent);
                
                // Add the method implementation
                updatedContent.Insert(insertPosition, Environment.NewLine + "    " + graphqlContent.Replace(Environment.NewLine, Environment.NewLine + "    ") + Environment.NewLine);
                
                // Add necessary using statements if they don't already exist
                string[] requiredNamespaces = new[]
                {
                    $"using {projectName}.Application.{(isQuery ? "Queries" : "Commands")}{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}.{operationName};",
                    $"using {projectName}.Application.Dtos{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")};"
                };
                
                foreach (var ns in requiredNamespaces)
                {
                    if (!existingContent.Contains(ns))
                    {
                        // Find position to insert namespaces (beginning of file)
                        int nsInsertPosition = existingContent.IndexOf("namespace");
                        if (nsInsertPosition > 0)
                        {
                            updatedContent.Insert(nsInsertPosition, ns + Environment.NewLine);
                        }
                    }
                }
                
                File.WriteAllText(graphqlFilePath, updatedContent.ToString());
            }
        }

        messenger.WriteStatusMessage(
            $"Created {(isQuery ? "query" : "command")} at {Path.GetRelativePath(projectDirectory, requestPath)}");
        messenger.WriteStatusMessage($"Created handler at {Path.GetRelativePath(projectDirectory, handlerPath)}");
        messenger.WriteStatusMessage($"Created response DTO at {Path.GetRelativePath(projectDirectory, responsePath)}");
        messenger.WriteStatusMessage($"{(isNewFile ? "Created" : "Updated")} GraphQL {(isQuery ? "query" : "mutation")} at {Path.GetRelativePath(projectDirectory, graphqlFilePath)}");

        return Result.Succeed();
    }
}
