using Apiand.TemplateEngine.Models;

namespace Apiand.TemplateEngine.Architectures.DDD;

public class DddTemplateConfiguration: TemplateConfiguration
{
    public required Option? Application { get; set; }
    public required Option? Domain { get; set; }
    public required Option? Presentation { get; set; }
    public required Option? Infrastructure { get; set; }
}