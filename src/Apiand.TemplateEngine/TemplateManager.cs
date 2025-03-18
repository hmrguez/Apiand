namespace Apiand.TemplateEngine;

public class TemplateManager
{
    private readonly string _templateBasePath;

    public TemplateManager(string? templateBasePath = null)
    {
        _templateBasePath = templateBasePath ?? Path.Combine(AppContext.BaseDirectory, "Templates");
    }

    public string? GetTemplatePath(string? templateId)
    {
        string path = templateId == null ? _templateBasePath : Path.Combine(_templateBasePath, templateId);
        return Directory.Exists(path) ? path : null;
    }

    public Dictionary<string, string> ResolveTemplates(TemplateConfiguration config)
    {
        var resolver = new TemplateResolver(GetTemplatePath(config.TemplatePath)!);
        return resolver.ResolveTemplatePaths(config);
    }

    public List<string> GetAvailableTemplates()
    {
        if (!Directory.Exists(_templateBasePath))
            return new List<string>();
        
        return Directory.GetDirectories(_templateBasePath)
            .Select(Path.GetFileName)
            .ToList()!;
    }
}

public class TemplateInfo
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Path { get; set; }
}