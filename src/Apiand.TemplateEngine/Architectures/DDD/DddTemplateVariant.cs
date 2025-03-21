using Apiand.TemplateEngine.Models;

namespace Apiand.TemplateEngine.Architectures.DDD;

public class DddTemplateVariant : TemplateVariant
{
    public required Layer Layer { get; init; }
    public required Option Option { get; init; }
}