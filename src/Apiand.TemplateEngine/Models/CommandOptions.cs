namespace Apiand.TemplateEngine.Models;

public class CommandOptions
{
    public string OutputPath { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? Presentation { get; set; } = string.Empty;
    public string? Infrastructure { get; set; } = string.Empty;
    public string? Application { get; set; }
    public string? Domain { get; set; }
}