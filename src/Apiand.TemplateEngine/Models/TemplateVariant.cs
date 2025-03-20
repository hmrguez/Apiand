namespace Apiand.TemplateEngine.Models;

public class TemplateVariant
{
    public required string BasePath { get; init; }
    public required string Architecture { get; init; }
    public string? ApiType { get; init; }
    public string? DbType { get; init; }
    public string? Module { get; init; }
    public bool IsDefault { get; init; }
}