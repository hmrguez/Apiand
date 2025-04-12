namespace Apiand.TemplateEngine.Models;

public class TemplateConfiguration
{
    public required string OutputPath { get; set; }
    public required string ProjectName { get; set; }
    public required string ArchName { get; set; }
    
    public static TemplateConfiguration Default(string workingDirectory) =>
        new()
        {
            ArchName = "Standalone",
            OutputPath = workingDirectory,
            ProjectName = Path.GetFileName(Path.GetFullPath(workingDirectory)),
        };
}