using Apiand.TemplateEngine.Constants;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Options;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine;

public class TemplateResolver
{
    private readonly List<TemplateVariant> _variants = new();
    private readonly string _baseTemplatePath;

    public TemplateResolver(string baseTemplatePath)
    {
        _baseTemplatePath = baseTemplatePath;
        LoadTemplateVariants();
    }

    private void LoadTemplateVariants()
    {
        foreach (var architecture in TemplateOptions.ArchitectureTypes)
        {
            string archPath = Path.Combine(_baseTemplatePath, architecture.Humanize());
            if (!Directory.Exists(archPath)) continue;

            foreach (var module in TemplateOptions.Modules)
            {
                string modulePath = Path.Combine(archPath, module.Humanize());
                if (!Directory.Exists(modulePath)) continue;

                // Add default variant
                _variants.Add(new TemplateVariant
                {
                    BasePath = Path.Combine(modulePath, "default"),
                    Architecture = architecture,
                    Module = module,
                    IsDefault = true
                });

                // Add API-specific variants for endpoints
                if (module is Module.Presentation)
                {
                    foreach (var apiType in TemplateOptions.EndpointTypes)
                    {
                        string apiPath = Path.Combine(modulePath, apiType.Humanize());
                        if (Directory.Exists(apiPath))
                        {
                            _variants.Add(new TemplateVariant
                            {
                                BasePath = apiPath,
                                Architecture = architecture,
                                Module = module,
                                ApiType = apiType,
                                IsDefault = false
                            });
                        }
                    }
                }

                // Add DB-specific variants for infrastructure
                if (module == Module.Infrastructure)
                {
                    foreach (var dbType in TemplateOptions.InfrastructureTypes)
                    {
                        string dbPath = Path.Combine(modulePath, dbType.Humanize());
                        if (Directory.Exists(dbPath))
                        {
                            _variants.Add(new TemplateVariant
                            {
                                BasePath = dbPath,
                                Architecture = architecture,
                                Module = module,
                                DbType = dbType,
                                IsDefault = false
                            });
                        }
                    }
                }
            }
        }
    }

    public Dictionary<Module, string> ResolveTemplatePaths(TemplateConfiguration config)
    {
        var modules = TemplateOptions.Modules;
        var result = new Dictionary<Module, string>();

        foreach (var module in modules)
        {
            // Try to find specific variant first
            var variant = _variants.FirstOrDefault(v => 
                v.Architecture == config.Architecture && 
                v.Module == module &&
                (v.ApiType == config.ApiType || v.DbType == config.DbType));

            // Fall back to default variant if specific one not found
            if (variant == null)
            {
                variant = _variants.FirstOrDefault(v =>
                    v.Architecture == config.Architecture &&
                    v.Module == module &&
                    v.IsDefault);
            }

            if (variant != null && Directory.Exists(variant.BasePath))
            {
                result[module] = variant.BasePath;
            }
        }

        return result;
    }
}