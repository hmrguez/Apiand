namespace Apiand.TemplateEngine;

public class TemplateManager
{
    private readonly string _templateBasePath = Path.Combine(AppContext.BaseDirectory, "Templates");

    public Dictionary<string, string> ResolveTemplates(TemplateConfiguration config)
    {
        var resolver = new TemplateResolver(_templateBasePath!);
        return resolver.ResolveTemplatePaths(config);
    }
}