using Apiand.TemplateEngine.Options;

namespace Apiand.TemplateEngine.Models;

public class TemplateVariant
{
    public required string BasePath { get; init; }
    public required Architecture Architecture { get; init; }
    public Presentation? ApiType { get; init; }
    public Infrastructure? DbType { get; init; }
    public Application? Application { get; init; }
    public Domain? Domain { get; init; }
    public Module? Module { get; init; }
    public bool IsDefault { get; init; }
}