using Apiand.TemplateEngine.Constants;
using Apiand.TemplateEngine.Models;

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
        // Scan the templates directory structure to find all variants
        // This could be based on folder structure or metadata files
        
        // Example structure:
        // templates/
        //   multi-layer/
        //     endpoints/
        //       default/
        //       graphql/
        //       rest/
        //     infrastructure/
        //       default/
        //       mongodb/
        //       sqlserver/
        //     application/
        //       default/
        //     domain/
        //       default/

        // For each variant found, add it to the _variants list
        // This is a simplified implementation - you'd need to actually scan directories
        foreach (var architecture in Options.ArchitectureTypes)
        {
            string archPath = Path.Combine(_baseTemplatePath, architecture);
            if (!Directory.Exists(archPath)) continue;

            foreach (var module in Options.Modules)
            {
                string modulePath = Path.Combine(archPath, module);
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
                if (module == "endpoints")
                {
                    foreach (var apiType in Options.EndpointTypes)
                    {
                        string apiPath = Path.Combine(modulePath, apiType);
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
                if (module == "infrastructure")
                {
                    foreach (var dbType in Options.InfrastructureTypes)
                    {
                        string dbPath = Path.Combine(modulePath, dbType);
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

    public Dictionary<string, string> ResolveTemplatePaths(TemplateConfiguration config)
    {
        var modules = Options.Modules;
        var result = new Dictionary<string, string>();

        foreach (var module in modules)
        {
            // Try to find specific variant first
            var variant = _variants.FirstOrDefault(v => 
                v.Architecture == config.Architecture && 
                v.Module == module &&
                ((v.ApiType == config.ApiType && !string.IsNullOrEmpty(v.ApiType)) || 
                 (v.DbType == config.DbType && !string.IsNullOrEmpty(v.DbType))));

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