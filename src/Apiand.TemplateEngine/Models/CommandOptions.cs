namespace Apiand.TemplateEngine.Models;

public class CommandOptions
{
    public string OutputPath { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? ApiType { get; set; } = string.Empty;
    public string? DbType { get; set; } = string.Empty;
    public string? Application { get; set; }
    public string? Domain { get; set; }
}