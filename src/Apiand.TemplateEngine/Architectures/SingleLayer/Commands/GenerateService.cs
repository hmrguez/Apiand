using Apiand.Extensions.Models;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Architectures.SingleLayer.Commands;

public class GenerateService : IGenerateService
{
    public string ArchName { get; set; } = SingleLayerUtils.Name;

    public Result Handle(string workingDirectory, string projectDir, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var normalizedName = argument;

        // Parse the service name for subdirectories (e.g., "UseCases.User" -> ["UseCases", "User"])
        string[] nameParts = normalizedName.Split('.');
        string serviceClassName = nameParts[^1]; // Last part is the actual service name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));

        // Find the main project
        string mainProject = null;

        // For SingleLayer architecture, find the main project
        var projectFiles = Directory.GetFiles(projectDir, "*.csproj", SearchOption.AllDirectories);

        // Get the first project file or the one matching the configuration project name
        foreach (var projectFile in projectFiles)
        {
            string projectFileName = Path.GetFileNameWithoutExtension(projectFile);
            if (projectFileName.Equals(configuration.ProjectName, StringComparison.OrdinalIgnoreCase))
            {
                mainProject = Path.GetDirectoryName(projectFile);
                break;
            }
            
            // If no specific match, just pick the first one
            if (mainProject == null)
            {
                mainProject = Path.GetDirectoryName(projectFile);
            }
        }

        if (mainProject == null)
        {
            return Result.Fail(TemplatingErrors.ProjectNotFound);
        }

        // Generate content using CodeBlocks
        string interfaceContent = CodeBlocks.GenerateServiceInterface(
            configuration.ProjectName, 
            serviceClassName, 
            subDirPath);

        string implementationContent = CodeBlocks.GenerateServiceImplementation(
            configuration.ProjectName, 
            serviceClassName, 
            subDirPath);

        // Create directories and files
        string servicesDir = Path.Combine(mainProject, "Services", subDirPath);
        Directory.CreateDirectory(servicesDir);

        string interfacePath = Path.Combine(servicesDir, $"I{serviceClassName}Service.cs");
        string implementationPath = Path.Combine(servicesDir, $"{serviceClassName}Service.cs");

        // Write files
        File.WriteAllText(interfacePath, interfaceContent);
        File.WriteAllText(implementationPath, implementationContent);

        messenger.WriteStatusMessage($"Created interface at {Path.GetRelativePath(projectDir, interfacePath)}");
        messenger.WriteStatusMessage($"Created implementation at {Path.GetRelativePath(projectDir, implementationPath)}");
        
        return Result.Succeed();
    }
}
