using System.CommandLine;
using Apiand.TemplateEngine;

namespace Apiand.Cli.Commands;

public class NewCommand : Command
{
    private readonly TemplateManager _templateManager;
    private readonly TemplateProcessor _processor;
    private readonly TemplateValidator _validator;

    public NewCommand() : base("new", "Creates a new project from a template")
    {
        _templateManager = new TemplateManager();
        _processor = new TemplateProcessor();
        _validator = new TemplateValidator();

        // Base options
        var templateOption = new Option<string>("--template", "The template to use") { IsRequired = false };
        templateOption.AddAlias("-t");

        var outputOption = new Option<string>("--output", "The output directory") { IsRequired = true };
        outputOption.AddAlias("-o");

        var nameOption = new Option<string>("--name", "Project name");
        nameOption.AddAlias("-n");

        // Architecture options
        var architectureOption = new Option<string>("--architecture",
            "Architecture type (single-layer, multi-layer, microservices)");
        architectureOption.AddAlias("-a");

        // API options
        var apiTypeOption = new Option<string>("--api-type", "API type (rest, graphql, grpc)");
        apiTypeOption.AddAlias("-api");

        // Database options
        var dbTypeOption = new Option<string>("--db-type", "Database type (mongodb, sqlserver, postgres, none)");
        dbTypeOption.AddAlias("-db");

        // Module options for multi-layer architectures
        var modulesOption = new Option<string[]>("--modules",
            "Modules to include (endpoints, application, infrastructure, domain)");
        modulesOption.AddAlias("-m");

        AddOption(templateOption);
        AddOption(outputOption);
        AddOption(nameOption);
        AddOption(architectureOption);
        AddOption(apiTypeOption);
        AddOption(dbTypeOption);
        AddOption(modulesOption);

        this.SetHandler(HandleCommand, templateOption, outputOption, nameOption, architectureOption, apiTypeOption,
            dbTypeOption, modulesOption);
    }

    private void HandleCommand(string? templateId, string output, string name, string architecture, string apiType,
        string dbType, string[] modules)
    {
        var templatePath = _templateManager.GetTemplatePath(templateId);
        if (templatePath == null)
        {
            Console.WriteLine($"Template '{templateId}' not found.");
            return;
        }

        // Create template configuration
        var config = new TemplateConfiguration
        {
            TemplatePath = templatePath,
            OutputPath = output,
            ProjectName = name ?? Path.GetFileName(Path.GetFullPath(output)),
            Architecture = architecture,
            ApiType = apiType,
            DbType = dbType,
            Modules = ["endpoints", "application", "infrastructure", "domain"]
        };

        // Validate configuration
        var validationResult = _validator.Validate(config);
        if (!validationResult.IsValid)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid configuration:");
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"- {error}");
            }

            Console.ResetColor();
            return;
        }
        
        // Resolve the template variants
        var templatePaths = _templateManager.ResolveTemplates(config);
        if (templatePaths.Count == 0)
        {
            Console.WriteLine("No matching templates found for the given configuration.");
            return;
        }

        // Process the template
        var data = new Dictionary<string, object>
        {
            ["name"] = config.ProjectName,
        };
        
        _processor.CreateFromTemplateVariants(templatePaths, output, data, config);
        Console.WriteLine($"Project created in {output}");
    }
}