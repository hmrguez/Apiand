using Apiand.Extensions.Models;
using Apiand.TemplateEngine.Ai;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Architectures.DDD.Commands;

public class GenerateService : IGenerateService
{
    public string ArchName { get; set; } = DddUtils.Name;

    public Result Handle(string workingDirectory, string projectDir, string argument,
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
            return Result.Fail(TemplatingErrors.ApplicationInfrastructureProjectsNotFound);
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

        var aiService = new AiService();
        if (extraData.TryGetValue("ai", out var ai))
        {
            interfaceContent = aiService.FillTemplateAsync(interfaceContent, "This is the service interface for the following prompt: " + ai).Result;
            implementationContent = aiService.FillTemplateAsync(implementationContent, "This is the service implementation for the following prompt: " + ai).Result;
        }

        // Create directories and files
        string interfaceDir = Path.Combine(applicationProject, "Services", subDirPath);
        string implementationDir = Path.Combine(infrastructureProject, "Services", subDirPath);

        Directory.CreateDirectory(interfaceDir);
        Directory.CreateDirectory(implementationDir);

        string interfacePath = Path.Combine(interfaceDir, $"I{serviceClassName}Service.cs");
        string implementationPath = Path.Combine(implementationDir, $"{serviceClassName}Service.cs");

        // Write files
        File.WriteAllText(interfacePath, interfaceContent);
        File.WriteAllText(implementationPath, implementationContent);

        messenger.WriteStatusMessage($"Created interface at {Path.GetRelativePath(projectDir, interfacePath)}");
        messenger.WriteStatusMessage($"Created implementation at {Path.GetRelativePath(projectDir, implementationPath)}");
        
        return Result.Succeed();
    }
}
