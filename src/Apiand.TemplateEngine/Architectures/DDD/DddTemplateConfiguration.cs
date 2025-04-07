using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Architectures.DDD;

[Architecture(DddUtils.Name)]
public class DddTemplateConfiguration: TemplateConfiguration
{
    public required Option? Application { get; set; }
    public required Option? Domain { get; set; }
    public required Option? Presentation { get; set; }
    public required Option? Infrastructure { get; set; }
    
    public required string DbType { get; set; }
}