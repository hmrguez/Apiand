using System.Diagnostics;
using Apiand.TemplateEngine.Models;

namespace Apiand.TemplateEngine.Architectures.SingleLayer;

public sealed class SingleLayer : ArchitectureType
{
    private string? _templatePath;
    public override string Name => SingleLayerUtils.Name;

    public override void LoadVariants(string basePath)
    {
        var archPath = Path.Combine(basePath, Name);

        if (!Directory.Exists(archPath))
            return;

        // Single Layer has only one variant, so we just need to store the path
        _templatePath = archPath;
    }

    public override TemplateConfiguration BuildConfig(CommandOptions commandOptions)
    {
        var config = new SingleLayerTemplateConfiguration
        {
            ArchName = Name,
            OutputPath = commandOptions.OutputPath,
            ProjectName = commandOptions.ProjectName ?? Path.GetFileName(Path.GetFullPath(commandOptions.OutputPath))
        };

        return config;
    }

    public override ValidationResult Validate(TemplateConfiguration configuration)
    {
        // No specific options to validate for SingleLayer
        return new ValidationResult();
    }

    public override Dictionary<string, string> Resolve(TemplateConfiguration configuration)
    {
        if (string.IsNullOrEmpty(_templatePath))
            return new Dictionary<string, string>();

        // Just one template path for SingleLayer
        return new Dictionary<string, string>
        {
            ["Api"] = _templatePath
        };
    }

    public override void ExecutePostCreationCommands(string outputPath, string projectName)
    {
        // No references to add for single layer architecture
    }
}
