namespace Apiand.TemplateEngine;

public class TemplateConfiguration
{
    public required string OutputPath { get; set; }
    public required string ProjectName { get; set; }
    public required string Architecture { get; set; }
    public required string ApiType { get; set; }
    public required string DbType { get; set; }
}