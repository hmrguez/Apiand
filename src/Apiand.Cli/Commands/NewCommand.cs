using System.CommandLine;
using Apiand.TemplateEngine;
using Apiand.TemplateEngine.Models;

namespace Apiand.Cli.Commands;

public class NewCommand : Command
{
    private readonly TemplateProcessor _processor;

    public NewCommand() : base("new", "Creates a new project from a template")
    {
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
        var arch = ArchitectureTypeFactory.Create(architecture);
        arch.LoadVariants(TemplateUtils.TemplatePath);

        var commandOptions = new CommandOptions()
        {
            OutputPath = output,
            ProjectName = name,
            Architecture = architecture,
            ApiType = apiType,
            DbType = dbType,
            Application = application,
            Domain = domain
        };
        
        var config = arch.BuildConfig(commandOptions);
        var validation = arch.Validate(config);
        if (!validation.IsValid)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid configuration:");
            foreach (var error in validation.Errors)
            {
                Console.WriteLine($"- {error}");
            }
        
            Console.ResetColor();
            return;
        }

        var templatePaths = arch.Resolve(config);
        if (templatePaths.Count == 0)
        {
            Console.WriteLine("No matching templates found for the given configuration.");
            return;
        }
        
        var data = new Dictionary<string, string>
        {
            ["name"] = config.ProjectName,
        };
        
        _processor.CreateFromTemplateVariants(templatePaths, output, data);
        Console.WriteLine($"Project created in {output}");
    }
}