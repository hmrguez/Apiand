using Apiand.TemplateEngine.Models;

namespace Apiand.TemplateEngine.Architectures.Microservices;

public sealed class Microservices : ArchitectureType
{
    private readonly List<string> _servicePaths = [];
    public override string Name => "Microservices";

    public override void LoadVariants(string basePath)
    {
        var archPath = Path.Combine(basePath, Name);
        var starterPath = Path.Combine(archPath, "Starter");

        if (!Directory.Exists(starterPath))
            return;

        // Load all service folders from the Starter directory
        foreach (var servicePath in Directory.GetDirectories(starterPath))
        {
            _servicePaths.Add(servicePath);
        }
    }

    public override TemplateConfiguration BuildConfig(CommandOptions commandOptions)
    {
        var config = new TemplateConfiguration
        {
            ArchName = Name,
            OutputPath = commandOptions.OutputPath,
            ProjectName = commandOptions.ProjectName ?? Path.GetFileName(Path.GetFullPath(commandOptions.OutputPath)),
        };

        return config;
    }

    public override ValidationResult Validate(TemplateConfiguration configuration)
    {
        return new ValidationResult();
    }

    public override Dictionary<string, string> Resolve(TemplateConfiguration configuration)
    {
        var result = new Dictionary<string, string>();
        
        if (_servicePaths.Count == 0)
            return result;

        // Add each service folder to the result
        foreach (var servicePath in _servicePaths)
        {
            var serviceName = Path.GetFileName(servicePath);
            result[serviceName] = servicePath;
        }

        return result;
    }

    public override void ExecutePostCreationCommands(string outputPath, string projectName)
    {
        
    }
}