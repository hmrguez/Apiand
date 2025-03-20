using System.CommandLine;
using Apiand.TemplateEngine;
using Apiand.TemplateEngine.Options;
using Apiand.TemplateEngine.Utils;

namespace Apiand.Cli.Commands;

public class NewCommand : Command
{
    private readonly TemplateManager _templateManager;
    private readonly TemplateProcessor _processor;

    public NewCommand() : base("new", "Creates a new project from a template")
    {
        _templateManager = new TemplateManager();
        _processor = new TemplateProcessor();

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
        
        // Application and domain options
        var applicationOption = new Option<string>("--application", "Application layer type (mvc, webapi, none)");
        applicationOption.AddAlias("-app");
        
        var domainOption = new Option<string>("--domain", "Domain layer type (entityframework, dapper, none)");
        domainOption.AddAlias("-dom");

        AddOption(outputOption);
        AddOption(nameOption);
        AddOption(architectureOption);
        AddOption(apiTypeOption);
        AddOption(dbTypeOption);
        AddOption(applicationOption);
        AddOption(domainOption);

        this.SetHandler(HandleCommand, outputOption, nameOption, architectureOption, apiTypeOption,
            dbTypeOption, applicationOption, domainOption);
    }

    private void HandleCommand(string output, string? name, string architecture, string apiType,
        string dbType, string? application, string? domain)
    {
        // Create template configuration
        var config = new TemplateConfiguration
        {
            OutputPath = output,
            ProjectName = name ?? Path.GetFileName(Path.GetFullPath(output)),
            Architecture = architecture.Dehumanize<Architecture>(),
            ApiType = apiType.Dehumanize<Presentation>(),
            DbType = dbType.Dehumanize<Infrastructure>(),
            Application = application?.Dehumanize<Application>() ?? Application.Default,
            Domain = domain?.Dehumanize<Domain>() ?? Domain.Default,
        };

        // Validate configuration
        var validationResult = TemplateValidator.Validate(config);
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
        var data = new Dictionary<string, string>
        {
            ["name"] = config.ProjectName,
        };
        
        _processor.CreateFromTemplateVariants(templatePaths, output, data, config);
        Console.WriteLine($"Project created in {output}");
    }
}