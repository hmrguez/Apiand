using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using Apiand.TemplateEngine.Utils;
using System;
using System.IO;
using System.Linq;

namespace Apiand.TemplateEngine.Architectures.SingleLayer.Commands;

public class GenerateEndpoint : IGenerateEndpoint
{
    public string ArchName { get; set; } = "SingleLayer";

    public void Handle(string workingDirectory, string projectDirectory, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var normalizedName = argument;

        // Parse the endpoint name for subdirectories (e.g., "Auth.User" -> ["Auth", "User"])
        string[] nameParts = normalizedName.Split('.');
        string endpointClassName = nameParts[^1]; // Last part is the actual endpoint name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));

        // Get HTTP method from extra data or default to GET
        string httpMethodStr = extraData.ContainsKey("http-method") ? extraData["http-method"] : "Get";
        bool isQuery = httpMethodStr.Equals("Get", StringComparison.OrdinalIgnoreCase);

        // Find the main project directory
        string mainProject = FindMainProject(projectDirectory);
        if (mainProject == null)
        {
            messenger.WriteErrorMessage("Could not find the main project for SingleLayer architecture.");
            return;
        }

        // For SingleLayer, we'll place endpoints directly in an Endpoints folder
        string endpointsDir = Path.Combine(mainProject, "Endpoints", subDirPath);
        Directory.CreateDirectory(endpointsDir);

        // Generate endpoint file content (containing request, response, and endpoint in a single file)
        string endpointContent = GenerateSingleFileEndpoint(
            configuration.ProjectName, endpointClassName, subDirPath, httpMethodStr, isQuery);

        // Write the endpoint file
        string endpointPath = Path.Combine(endpointsDir, $"{endpointClassName}Endpoint.cs");
        File.WriteAllText(endpointPath, endpointContent);

        messenger.WriteStatusMessage(
            $"Created {endpointClassName} endpoint at {Path.GetRelativePath(projectDirectory, endpointPath)}");
    }

    private string FindMainProject(string projectDirectory)
    {
        // For a single-layer architecture, we're looking for the main project
        // which could be the only project or one that doesn't have typical layer suffixes
        var projectFiles = Directory.GetFiles(projectDirectory, "*.csproj", SearchOption.AllDirectories);

        if (projectFiles.Length == 1)
        {
            // If there's only one project, it must be the main one
            return Path.GetDirectoryName(projectFiles[0]);
        }

        // Try to find the main project by naming convention
        foreach (var projectFile in projectFiles)
        {
            string projectName = Path.GetFileNameWithoutExtension(projectFile);

            // Skip projects that seem like they might be supporting projects
            if (!projectName.EndsWith("Tests") &&
                !projectName.EndsWith("Infrastructure") &&
                !projectName.EndsWith("Domain"))
            {
                return Path.GetDirectoryName(projectFile);
            }
        }

        // Fallback to the first project if we couldn't find a suitable one
        return projectFiles.Length > 0 ? Path.GetDirectoryName(projectFiles[0]) : null;
    }

    private string GenerateSingleFileEndpoint(string projectName, string endpointName, string subdirectory,
        string httpMethod, bool isQuery)
    {
        string namespacePath = string.IsNullOrEmpty(subdirectory)
            ? $"{projectName}.Endpoints"
            : $"{projectName}.Endpoints.{subdirectory.Replace("/", ".")}";

        string httpMethodLower = httpMethod.ToLowerInvariant();
        string route =
            $"/api/{(string.IsNullOrEmpty(subdirectory) ? "" : subdirectory.ToLowerInvariant().Replace("/", "/"))}/{endpointName.ToLowerInvariant()}";

        return 
            $$"""
            using FastEndpoints;
            using System.Threading;
            using System.Threading.Tasks;
            
            namespace {{namespacePath}};
            
            public class {{endpointName}}Request
            {
                // TODO: Define your request properties here
            }
            
            public class {{endpointName}}Response
            {
                public bool Success { get; set; }
                public string Message { get; set; } = string.Empty;
                // TODO: Define your response properties here
            }
            
            public class {{endpointName}}Endpoint : Endpoint<{{endpointName}}Request, {{endpointName}}Response>
            {
                public override void Configure()
                {
                    {{httpMethod}}("{{route}}");
                    AllowAnonymous(); // Modify as needed for authentication
                    Summary(s => {
                        s.Summary = "{{endpointName}} endpoint";
                        s.Description = "Description for {{endpointName}} endpoint";
                    });
                }
                
                public override async Task HandleAsync({{endpointName}}Request req, CancellationToken ct)
                {
                    // TODO: Implement your endpoint logic here
                    
                    await SendAsync(new {{endpointName}}Response
                    {
                        Success = true,
                        Message = "{{endpointName}} operation completed"
                    }, cancellation: ct);
                }
            }
            """;
    }
}