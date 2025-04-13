using System.CommandLine;
using Apiand.TemplateEngine.Architectures.DDD;
using Apiand.TemplateEngine.Models;
using Spectre.Console;

namespace Apiand.Cli.Commands.New;

public class NewDddCommand : NewCommand
{
    public NewDddCommand() : base("ddd", "Creates a new DDD project")
    {
        // Shared Options
        var outputOption = new Option<string>("--output", "The output directory");
        outputOption.AddAlias("-o");

        var nameOption = new Option<string>("--name", "Project name") { IsRequired = true };
        nameOption.AddAlias("-n");

        var presentationOption = new Option<string>("--presentation", "API type (FastEndpoints)");
        presentationOption.AddAlias("-pre");

        var infraOption = new Option<string>("--infrastructure", "Database type (MongoDB, EF Core)");
        infraOption.AddAlias("-inf");

        var applicationOption = new Option<string>("--application", "Application layer type (MediatR)");
        applicationOption.AddAlias("-app");

        var domainOption = new Option<string>("--domain", "Domain layer type (Default)");
        domainOption.AddAlias("-dom");

        var interactiveOption = new Option<bool>("--skip-interactive", "Don't use interactive for missing options");
        interactiveOption.AddAlias("-si");

        var dbTypeOption = new Option<string>("--db-type", "Database type (MongoDB, SQLServer, PostgreSQL)");
        dbTypeOption.AddAlias("-db");

        AddOption(outputOption);
        AddOption(nameOption);
        AddOption(presentationOption);
        AddOption(infraOption);
        AddOption(applicationOption);
        AddOption(domainOption);
        AddOption(interactiveOption);

        this.SetHandler(HandleCommand, outputOption, nameOption, presentationOption, infraOption, applicationOption,
            domainOption, interactiveOption, dbTypeOption);
    }

    private void HandleCommand(string? output, string name, string? presentation, string? infra, string? application,
        string? domain, bool skipInteractive, string? dbType)
    {
        // Define available options for each layer
        var presentationOptions = new[] { "FastEndpoints, GraphQL" };
        var infrastructureOptions = new[] { "MongoDB", "EFCore" };
        var dbTypeOptions = new[] { "SQLServer", "PostgreSQL" };
        var applicationOptions = new[] { "MediatR" };
        var domainOptions = new[] { "Default" };


        // If interactive mode or required options are missing, use Spectre.Console selectors
        if (!skipInteractive && string.IsNullOrEmpty(presentation))
        {
            presentation = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Presentation[/] layer type:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(presentationOptions));
        }

        if (!skipInteractive && string.IsNullOrEmpty(infra))
        {
            infra = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Infrastructure[/] layer type:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(infrastructureOptions));
        }

        if (!skipInteractive && string.IsNullOrEmpty(application))
        {
            application = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Application[/] layer type:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(applicationOptions));
        }

        if (!skipInteractive && string.IsNullOrEmpty(domain))
        {
            domain = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]Domain[/] layer type:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(domainOptions));
        }

        if (string.IsNullOrEmpty(dbType))
        {
            if (infra == "MongoDB")
            {
                dbType = "mongodb";
            }
            else
            {
                if (skipInteractive)
                    dbType = "PostgreSQL";
                else
                    dbType = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Select [green]Database[/] type:")
                                .PageSize(10)
                                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                                .AddChoices(dbTypeOptions))
                        .ToLower();
            }
        }

        var commandOptions = new CommandOptions
        {
            OutputPath = output ?? "./" + name,
            ProjectName = name,
            Presentation = presentation,
            Infrastructure = infra,
            Application = application,
            Domain = domain,
            DbType = dbType
        };

        var extraOptions = new Dictionary<string, string>
        {
            { "db", dbType }
        };

        HandleArchCommand(new DDD(), commandOptions, extraOptions);
    }
}