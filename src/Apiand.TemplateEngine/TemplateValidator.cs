using Apiand.TemplateEngine.Constants;
using Apiand.TemplateEngine.Options;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine;

public static class TemplateValidator
{
    public static ValidationResult Validate(TemplateConfiguration config)
    {
        var result = new ValidationResult();

        return config.Architecture switch
        {
            null => result.AddError("Architecture must be specified"),

            var arch when !EnumUtils.GetAll<Architecture>().Contains(arch.Value) =>
                result.AddError($"Invalid architecture '{arch.Value.Humanize()}'. Valid options: " +
                                $"{string.Join(", ", EnumUtils.GetAll<Architecture>())}"),

            Architecture.MultiLayer when config.ApiType == null || config.DbType == null =>
                result.AddErrors(
                    config.ApiType == null ? "API type must be specified for Multi-Layer architecture" : null,
                    config.DbType == null ? "Database type must be specified for Multi-Layer architecture" : null),

            var arch => ValidateApiAndDbTypes(result, arch.Value, config.ApiType, config.DbType)
        };
    }

    private static ValidationResult ValidateApiAndDbTypes(ValidationResult result, Architecture architecture,
        Endpoint? apiType, Infrastructure? dbType)
    {
        if (apiType.HasValue &&
            TemplateOptions.ValidEndpointTypes.TryGetValue(architecture, out var validApiTypes) &&
            !validApiTypes.Contains(apiType.Value))
        {
            result.AddError(
                $"API type '{apiType.Value.Humanize()}' is not valid for architecture '{architecture.Humanize()}'. " +
                $"Valid options: {string.Join(", ", validApiTypes.Select(v => v.Humanize()))}");
        }

        if (dbType.HasValue &&
            TemplateOptions.ValidDbTypes.TryGetValue(architecture, out var validDbTypes) &&
            !validDbTypes.Contains(dbType.Value))
        {
            result.AddError(
                $"Database type '{dbType.Value.Humanize()}' is not valid for architecture '{architecture.Humanize()}'. " +
                $"Valid options: {string.Join(", ", validDbTypes.Select(v => v.Humanize()))}");
        }

        return result;
    }

    private static ValidationResult AddError(this ValidationResult result, string error)
    {
        result.Errors.Add(error);
        return result;
    }

    private static ValidationResult AddErrors(this ValidationResult result, params string?[] error)
    {
        foreach (var err in error.Where(e => e != null))
            result.Errors.Add(err!);
        return result;
    }
}