namespace Apiand.TemplateEngine.Models;

public abstract class ArchitectureType
{
    /// <summary>
    /// Name for the architecture, must match the folder name in the templates folder. Preferably lowercase.
    /// </summary>
    public abstract string Name { get; }
    public abstract void LoadVariants(string basePath);
    public abstract TemplateConfiguration BuildConfig(CommandOptions commandOptions);
    public abstract ValidationResult Validate(TemplateConfiguration configuration);
    public abstract Dictionary<string, string> Resolve(TemplateConfiguration configuration);
}