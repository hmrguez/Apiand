using Apiand.TemplateEngine.Models;

namespace Apiand.TemplateEngine.Architectures.Standalone;

public class Standalone: ArchitectureType
{
    public override string Name { get; } = "Standalone";
    public override void LoadVariants(string basePath)
    {
        throw new NotImplementedException();
    }

    public override TemplateConfiguration BuildConfig(CommandOptions commandOptions)
    {
        throw new NotImplementedException();
    }

    public override ValidationResult Validate(TemplateConfiguration configuration)
    {
        throw new NotImplementedException();
    }

    public override Dictionary<string, string> Resolve(TemplateConfiguration configuration)
    {
        throw new NotImplementedException();
    }

    public override void ExecutePostCreationCommands(string outputPath, string projectName)
    {
        throw new NotImplementedException();
    }
}