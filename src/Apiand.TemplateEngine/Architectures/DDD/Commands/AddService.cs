using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Architectures.DDD.Commands;

public class AddService : IAddService
{
    public string ArchName { get; set; } = DddUtils.Name;

    public void Handle(string workingDirectory, string projectDir, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var normalizedName = argument;

        // Parse the service name for subdirectories (e.g., "UseCases.User" -> ["UseCases", "User"])
        string[] nameParts = normalizedName.Split('.');
        string serviceClassName = nameParts[^1]; // Last part is the actual service name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));

        // Find the appropriate projects for interface and implementation

        string applicationProject = null;
        string infrastructureProject = null;

        // For DDD architecture, find Application and Infrastructure projects
        var projectFiles = Directory.GetFiles(projectDir, "*.csproj", SearchOption.AllDirectories);

        foreach (var projectFile in projectFiles)
        {
            string projectFileName = Path.GetFileNameWithoutExtension(projectFile);
            if (projectFileName.EndsWith("Application"))
            {
                applicationProject = Path.GetDirectoryName(projectFile);
            }
            else if (projectFileName.EndsWith("Infrastructure"))
            {
                infrastructureProject = Path.GetDirectoryName(projectFile);
            }
        }

        if (applicationProject == null || infrastructureProject == null)
        {
            messenger.WriteErrorMessage(
                "Could not find required Application and Infrastructure projects in DDD architecture.");
            return;
        }

        // Create interface and implementation templates
        string interfaceContent =
            $$"""
              namespace {{configuration.ProjectName}}.Application.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
                      
              public interface I{{argument}}Service
              {
                  // TODO: Add service methods
              }
              """;

        string implementationContent =
            $$"""
              using {{configuration.ProjectName}}.Application.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

              namespace {{configuration.ProjectName}}.Infrastructure.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

              public class {{argument}}Service : I{{argument}}Service
              {
                  // TODO: Implement service methods
              }
              """;

        // Create directories and files
        string interfaceDir = Path.Combine(applicationProject, "Services", subDirPath);
        string implementationDir = Path.Combine(infrastructureProject, "Services", subDirPath);

        Directory.CreateDirectory(interfaceDir);
        Directory.CreateDirectory(implementationDir);

        string interfacePath = Path.Combine(interfaceDir, $"I{serviceClassName}Service.cs");
        string implementationPath = Path.Combine(implementationDir, $"{serviceClassName}Service.cs");

        // Process the content using the template engine if needed
        var interfaceTemplateData = new Dictionary<string, string>(extraData);
        var implementationTemplateData = new Dictionary<string, string>(extraData);

        interfaceTemplateData["serviceNamespace"] =
            $"{configuration.ProjectName}.Application.Services{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";
        implementationTemplateData["serviceNamespace"] =
            $"{configuration.ProjectName}.Infrastructure.Services{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";
        interfaceTemplateData["interfaceNamespace"] = interfaceTemplateData["serviceNamespace"];

        // Write files
        File.WriteAllText(interfacePath, interfaceContent);
        File.WriteAllText(implementationPath, implementationContent);

        messenger.WriteStatusMessage($"Created interface at {Path.GetRelativePath(projectDir, interfacePath)}");
        messenger.WriteStatusMessage($"Created implementation at {Path.GetRelativePath(projectDir, implementationPath)}");

        // After creating interface and implementation files
        // For DDD architecture, register the service in DI
        string serviceNamespace =
            $"{configuration.ProjectName}.Application.Services{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";
        RoslynUtils.RegisterServiceInModule(infrastructureProject, serviceClassName, serviceNamespace, messenger);
    }
}