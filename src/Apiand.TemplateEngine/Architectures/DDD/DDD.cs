using System.Diagnostics;
using Apiand.Extensions.Utils;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Architectures.DDD;

public sealed class DDD : ArchitectureType
{
    private readonly List<DddTemplateVariant> _variants = [];
    public override string Name => DddUtils.Name;

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
        var temp = new DddTemplateConfiguration
        {
            Application = GetOrDefault(commandOptions.Application, Layer.Application),
            ArchName = Name,
            Domain = GetOrDefault(commandOptions.Domain, Layer.Domain),
            Presentation = GetOrDefault(commandOptions.Presentation, Layer.Presentation),
            Infrastructure = GetOrDefault(commandOptions.Infrastructure, Layer.Infrastructure),
            OutputPath = commandOptions.OutputPath,
            ProjectName = commandOptions.ProjectName ?? Path.GetFileName(Path.GetFullPath(commandOptions.OutputPath)),
            DbType = commandOptions.DbType
        };


        return temp;
    }

    private static Option? GetOrDefault(string? value, Layer layer)
    {
        return string.IsNullOrEmpty(value) ? DddOptions.GetByLayer(layer).FirstOrDefault() : value.Dehumanize<Option>();
    }

    public override ValidationResult Validate(TemplateConfiguration configuration)
    {
        var dddConfig = configuration as DddTemplateConfiguration;
        if (dddConfig == null)
            throw new InvalidOperationException("Invalid flow");

        var result = new ValidationResult();

        // Validate Presentation option
        if (dddConfig.Presentation == null)
            result.AddError("A valid API type must be specified for DDD architecture");
        else if (!DddOptions.GetByLayer(Layer.Presentation).Contains(dddConfig.Presentation.Value))
            result.AddError(
                $"API type '{dddConfig.Presentation.Value.Humanize()}' is not valid for DDD architecture. " +
                $"Valid options: {string.Join(", ", DddOptions.GetByLayer(Layer.Presentation).Select(v => v.Humanize()))}");

        // Validate Infrastructure option
        if (dddConfig.Infrastructure == null)
            result.AddError("A valid Database type must be specified for DDD architecture");
        else if (!DddOptions.GetByLayer(Layer.Infrastructure).Contains(dddConfig.Infrastructure.Value))
            result.AddError(
                $"Database type '{dddConfig.Infrastructure.Value.Humanize()}' is not valid for DDD architecture. " +
                $"Valid options: {string.Join(", ", DddOptions.GetByLayer(Layer.Infrastructure).Select(v => v.Humanize()))}");

        // Validate Application option
        if (dddConfig.Application == null)
            result.AddError("A valid Application type must be specified for DDD architecture");
        else if (!DddOptions.GetByLayer(Layer.Application).Contains(dddConfig.Application.Value))
            result.AddError(
                $"Application type '{dddConfig.Application.Value.Humanize()}' is not valid for DDD architecture. " +
                $"Valid options: {string.Join(", ", DddOptions.GetByLayer(Layer.Application).Select(v => v.Humanize()))}");

        // Validate Domain option
        if (dddConfig.Domain == null)
            result.AddError("A valid Domain type must be specified for DDD architecture");
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
        if (_variants.Count == 0)
            return result;
        
        result[Layer.Application.Humanize()] = GetMatchingVariantPath(Layer.Application, config.Application);
        result[Layer.Domain.Humanize()] = GetMatchingVariantPath(Layer.Domain, config.Domain);
        result[Layer.Infrastructure.Humanize()] = GetMatchingVariantPath(Layer.Infrastructure, config.Infrastructure);
        result[Layer.Presentation.Humanize()] = GetMatchingVariantPath(Layer.Presentation, config.Presentation);
        
        // Add special projects
        result[Layer.AppHost.Humanize()] = GetMatchingVariantPath(Layer.AppHost, Option.Default);
        result[Layer.ServiceDefaults.Humanize()] = GetMatchingVariantPath(Layer.ServiceDefaults, Option.Default);

        return result;
    }

    private string GetMatchingVariantPath(Layer layer, Option? option)
    {
        var match = 
            _variants.FirstOrDefault(v => v.Layer == layer && v.Option == option) ??
            _variants.FirstOrDefault(v => v.Layer == layer && v.IsDefault);

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
    
    public override void ExecutePostCreationCommands(string outputPath, string projectName)
    {
        var projectFiles = Directory.GetFiles(outputPath, "*.csproj", SearchOption.AllDirectories);
        
        var projectPaths = new Dictionary<Layer, string>();
        var specialProjects = new Dictionary<string, string>();
        
        foreach (var projectFile in projectFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(projectFile);
            
            if (fileName.EndsWith(".AppHost", StringComparison.OrdinalIgnoreCase) || 
                fileName.EndsWith(".ServiceDefaults", StringComparison.OrdinalIgnoreCase))
            {
                specialProjects[fileName] = projectFile;
                continue;
            }
            
            foreach (Layer layer in Enum.GetValues(typeof(Layer)))
            {
                string layerName = layer.ToString();
                if (fileName.EndsWith(layerName, StringComparison.OrdinalIgnoreCase))
                {
                    projectPaths[layer] = projectFile;
                    break;
                }
            }
        }
        
        if (projectPaths.Count == 0 && specialProjects.Count == 0)
            return;
        
        // 1. Application references Domain
        if (projectPaths.TryGetValue(Layer.Application, out var appProject) && 
            projectPaths.TryGetValue(Layer.Domain, out var domainProject))
        {
            AddProjectReference(outputPath, appProject, domainProject);
        }
        
        // 2. Infrastructure references Domain and Application
        if (projectPaths.TryGetValue(Layer.Infrastructure, out var infraProject))
        {
            if (projectPaths.TryGetValue(Layer.Domain, out var domainProject2))
            {
                AddProjectReference(outputPath, infraProject, domainProject2);
            }
            
            if (projectPaths.TryGetValue(Layer.Application, out var appProject2))
            {
                AddProjectReference(outputPath, infraProject, appProject2);
            }
        }
        
        // 3. Presentation (API) references Application and Infrastructure
        if (projectPaths.TryGetValue(Layer.Presentation, out var apiProject))
        {
            if (projectPaths.TryGetValue(Layer.Application, out var appProject3))
            {
                AddProjectReference(outputPath, apiProject, appProject3);
            }
            
            if (projectPaths.TryGetValue(Layer.Infrastructure, out var infraProject2))
            {
                AddProjectReference(outputPath, apiProject, infraProject2);
            }
        }
        
        // 4. Handle AppHost and ServiceDefaults projects
        var appHostProject = specialProjects.FirstOrDefault(p => p.Key.EndsWith(".AppHost", StringComparison.OrdinalIgnoreCase)).Value;
        var serviceDefaultsProject = specialProjects.FirstOrDefault(p => p.Key.EndsWith(".ServiceDefaults", StringComparison.OrdinalIgnoreCase)).Value;
        
        if (!string.IsNullOrEmpty(appHostProject) && !string.IsNullOrEmpty(serviceDefaultsProject))
        {
            AddProjectReference(outputPath, appHostProject, apiProject);
            AddProjectReference(outputPath, apiProject, serviceDefaultsProject);
            AddProjectReference(outputPath, appHostProject, serviceDefaultsProject);
        }
    }
    
    private void AddProjectReference(string workingDirectory, string sourceProject, string targetProject)
    {
        // Convert to relative path
        sourceProject = sourceProject.Replace(workingDirectory, "./");
        targetProject = targetProject.Replace(workingDirectory, "./");
        
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"add \"{sourceProject}\" reference \"{targetProject}\"",
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
    
        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}
