namespace Apiand.TemplateEngine.Models;

public class TemplateMetadata
{
    public string Name { get; set; } = "Unnamed Template";
    public string Description { get; set; } = "";
    public List<string> TextFileExtensions { get; set; } = [".cs", ".json", ".xml", ".csproj", ".txt", ".md"];
    public List<string> ExcludeFiles { get; init; } = [];
    public List<string> ExcludeDirs { get; init; } = [];
    public List<string> PostActions { get; init; } = [];
}