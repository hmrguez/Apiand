using Apiand.TemplateEngine.Options;

namespace Apiand.TemplateEngine.Models;

public class TemplateVariant
{
    public required string BasePath { get; init; }
    public required Architecture Architecture { get; init; }
    public Endpoint? ApiType { get; init; }
    public Infrastructure? DbType { get; init; }
    public Module? Module { get; init; }
    public bool IsDefault { get; init; }
}