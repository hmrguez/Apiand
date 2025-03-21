using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Architectures.DDD;

public sealed class DDD : ArchitectureType
{
    private readonly List<DddTemplateVariant> _variants = [];
    public override string Name => "ddd";

    public override void LoadVariants(string basePath)
    {
        var archPath = Path.Combine(basePath, Name);

        if (!Directory.Exists(archPath))
            return;

        var layers = EnumUtils.GetAll<Layer>();

        foreach (var layer in layers) LoadLayerVariants(archPath, layer);
    }

    public override TemplateConfiguration BuildConfig(CommandOptions commandOptions)
    {
        return new DddTemplateConfiguration
        {
            Application = GetOrDefault(commandOptions.Application),
            Domain = GetOrDefault(commandOptions.Domain),
            Presentation = GetOrDefault(commandOptions.ApiType),
            Infrastructure = GetOrDefault(commandOptions.DbType),
            OutputPath = commandOptions.OutputPath,
            ProjectName = commandOptions.ProjectName ?? Path.GetFileName(Path.GetFullPath(commandOptions.OutputPath))
        };
    }

    private static Option? GetOrDefault(string? value)
    {
        return string.IsNullOrEmpty(value) ? Option.Default : value.Dehumanize<Option>();
    }

    public override ValidationResult Validate(TemplateConfiguration configuration)
    {
        var dddConfig = configuration as DddTemplateConfiguration;
        if (dddConfig == null)
            throw new InvalidOperationException("Invalid flow");

        var result = new ValidationResult();

        // Validate Presentation option
        if (dddConfig.Presentation == null)
            result.AddError("API type must be specified for DDD architecture");
        else if (!DddOptions.GetByLayer(Layer.Presentation).Contains(dddConfig.Presentation.Value))
            result.AddError(
                $"API type '{dddConfig.Presentation.Value.Humanize()}' is not valid for DDD architecture. " +
                $"Valid options: {string.Join(", ", DddOptions.GetByLayer(Layer.Presentation).Select(v => v.Humanize()))}");

        // Validate Infrastructure option
        if (dddConfig.Infrastructure == null)
            result.AddError("Database type must be specified for DDD architecture");
        else if (!DddOptions.GetByLayer(Layer.Infrastructure).Contains(dddConfig.Infrastructure.Value))
            result.AddError(
                $"Database type '{dddConfig.Infrastructure.Value.Humanize()}' is not valid for DDD architecture. " +
                $"Valid options: {string.Join(", ", DddOptions.GetByLayer(Layer.Infrastructure).Select(v => v.Humanize()))}");

        // Validate Application option
        if (dddConfig.Application == null)
            result.AddError("Application type must be specified for DDD architecture");
        else if (!DddOptions.GetByLayer(Layer.Application).Contains(dddConfig.Application.Value))
            result.AddError(
                $"Application type '{dddConfig.Application.Value.Humanize()}' is not valid for DDD architecture. " +
                $"Valid options: {string.Join(", ", DddOptions.GetByLayer(Layer.Application).Select(v => v.Humanize()))}");

        // Validate Domain option
        if (dddConfig.Domain == null)
            result.AddError("Domain type must be specified for DDD architecture");
        else if (!DddOptions.GetByLayer(Layer.Domain).Contains(dddConfig.Domain.Value))
            result.AddError($"Domain type '{dddConfig.Domain.Value.Humanize()}' is not valid for DDD architecture. " +
                            $"Valid options: {string.Join(", ", DddOptions.GetByLayer(Layer.Domain).Select(v => v.Humanize()))}");

        return result;
    }

    public override Dictionary<string, string> Resolve(TemplateConfiguration configuration)
    {
        var config = configuration as DddTemplateConfiguration;
        if (config == null)
            throw new InvalidOperationException("Invalid flow");

        var result = new Dictionary<string, string>();
        if (!_variants.Any())
            return result;

        result[Layer.Application.Humanize()] = GetMatchingVariantPath(Layer.Application, config.Application);
        result[Layer.Domain.Humanize()] = GetMatchingVariantPath(Layer.Domain, config.Domain);
        result[Layer.Infrastructure.Humanize()] = GetMatchingVariantPath(Layer.Infrastructure, config.Infrastructure);
        result[Layer.Presentation.Humanize()] = GetMatchingVariantPath(Layer.Presentation, config.Presentation);

        return result;
    }

    private string GetMatchingVariantPath(Layer layer, Option? option)
    {
        var match = _variants.FirstOrDefault(v => v.Layer == layer && v.Option == option);
        if (match == null)
            match = _variants.FirstOrDefault(v => v.Layer == layer && v.IsDefault);

        return match!.BasePath;
    }

    private void LoadLayerVariants(string archPath, Layer layer)
    {
        var modulePath = Path.Combine(archPath, layer.Humanize());
        if (!Directory.Exists(modulePath))
            return;

        var isFirstVariantForModule = true;

        foreach (var option in DddOptions.GetByLayer(layer))
        {
            var apiPath = Path.Combine(modulePath, option.Humanize());
            if (Directory.Exists(apiPath))
            {
                _variants.Add(new DddTemplateVariant
                {
                    BasePath = apiPath,
                    Layer = layer,
                    Option = option,
                    IsDefault = isFirstVariantForModule
                });
                isFirstVariantForModule = false;
            }
        }
    }
}