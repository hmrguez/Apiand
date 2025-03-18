namespace Apiand.TemplateEngine;

public class TemplateConfiguration
{
    public string? TemplatePath { get; set; }
    public required string OutputPath { get; set; }
    public required string ProjectName { get; set; }
    public required string Architecture { get; set; }
    public required string ApiType { get; set; }
    public required string DbType { get; set; }
    public List<string> Modules { get; set; } = new();
}

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new();
}

public class TemplateValidator
{
    private readonly Dictionary<string, List<string>> _requiredModules = new()
    {
        ["multi-layer"] = ["domain"],
        ["microservices"] = ["domain", "infrastructure"]
    };

    private readonly Dictionary<string, List<string>> _validApiTypes = new()
    {
        ["single-layer"] = ["rest", "graphql", "grpc", "none"],
        ["multi-layer"] = ["rest", "graphql", "grpc", "none"],
        ["microservices"] = ["rest", "graphql", "grpc"]
    };

    private readonly Dictionary<string, List<string>> _validDbTypes = new()
    {
        ["single-layer"] = ["mongodb", "sqlserver", "postgres", "none"],
        ["multi-layer"] = ["mongodb", "sqlserver", "postgres", "none"],
        ["microservices"] = ["mongodb", "sqlserver", "postgres", "none"]
    };

    private readonly List<string> _validModules = ["endpoints", "application", "infrastructure", "domain"];

    public ValidationResult Validate(TemplateConfiguration config)
    {
        var result = new ValidationResult();

        // Validate architecture
        if (!string.IsNullOrEmpty(config.Architecture) &&
            !_validApiTypes.ContainsKey(config.Architecture))
            result.Errors.Add(
                $"Invalid architecture '{config.Architecture}'. Valid options: {string.Join(", ", _validApiTypes.Keys)}");

        // If architecture is specified, validate API type
        if (!string.IsNullOrEmpty(config.Architecture) &&
            !string.IsNullOrEmpty(config.ApiType) &&
            _validApiTypes.TryGetValue(config.Architecture, out var validApiTypes) &&
            !validApiTypes.Contains(config.ApiType))
            result.Errors.Add(
                $"API type '{config.ApiType}' is not valid for architecture '{config.Architecture}'. Valid options: {string.Join(", ", validApiTypes)}");

        // If architecture is specified, validate database type
        if (!string.IsNullOrEmpty(config.Architecture) &&
            !string.IsNullOrEmpty(config.DbType) &&
            _validDbTypes.TryGetValue(config.Architecture, out var validDbTypes) &&
            !validDbTypes.Contains(config.DbType))
            result.Errors.Add(
                $"Database type '{config.DbType}' is not valid for architecture '{config.Architecture}'. Valid options: {string.Join(", ", validDbTypes)}");

        // Validate modules
        if (config.Modules is not { Count: > 0 }) return result;
        
        // Check if all specified modules are valid
        var invalidModules = config.Modules.Where(m => !_validModules.Contains(m)).ToList();
        if (invalidModules.Any())
            result.Errors.Add(
                $"Invalid module(s): {string.Join(", ", invalidModules)}. Valid modules: {string.Join(", ", _validModules)}");

        // Check if required modules are included based on architecture
        if (!string.IsNullOrEmpty(config.Architecture) &&
            _requiredModules.TryGetValue(config.Architecture, out var requiredModules))
        {
            var missingModules = requiredModules.Where(m => !config.Modules.Contains(m)).ToList();
            if (missingModules.Any())
                result.Errors.Add(
                    $"Architecture '{config.Architecture}' requires the following modules: {string.Join(", ", missingModules)}");
        }

        return result;
    }
}