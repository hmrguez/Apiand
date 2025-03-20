using Apiand.TemplateEngine.Constants;

namespace Apiand.TemplateEngine;

public class TemplateValidator
{
    public ValidationResult Validate(TemplateConfiguration config)
    {
        var result = new ValidationResult();

        // Validate architecture
        if (!string.IsNullOrEmpty(config.Architecture) &&
            !Options.ValidEndpointTypes.ContainsKey(config.Architecture))
            result.Errors.Add(
                $"Invalid architecture '{config.Architecture}'. Valid options: {string.Join(", ", Options.ValidEndpointTypes.Keys)}");

        // If architecture is specified, validate API type
        if (!string.IsNullOrEmpty(config.Architecture) &&
            !string.IsNullOrEmpty(config.ApiType) &&
            Options.ValidEndpointTypes.TryGetValue(config.Architecture, out var validApiTypes) &&
            !validApiTypes.Contains(config.ApiType))
            result.Errors.Add(
                $"API type '{config.ApiType}' is not valid for architecture '{config.Architecture}'. Valid options: {string.Join(", ", validApiTypes)}");

        // If architecture is specified, validate database type
        if (!string.IsNullOrEmpty(config.Architecture) &&
            !string.IsNullOrEmpty(config.DbType) &&
            Options.ValidDbTypes.TryGetValue(config.Architecture, out var validDbTypes) &&
            !validDbTypes.Contains(config.DbType))
            result.Errors.Add(
                $"Database type '{config.DbType}' is not valid for architecture '{config.Architecture}'. Valid options: {string.Join(", ", validDbTypes)}");

        return result;
    }
}