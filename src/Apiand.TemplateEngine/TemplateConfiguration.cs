using Apiand.TemplateEngine.Options;

namespace Apiand.TemplateEngine;

public class TemplateConfiguration
{
    public required string OutputPath { get; set; }
    public required string ProjectName { get; set; }
    public required Architecture? Architecture { get; set; }
    public required Presentation? ApiType { get; set; }
    public required Infrastructure? DbType { get; set; }
    public required Application? Application { get; set; }
    public required Domain? Domain { get; set; }
}