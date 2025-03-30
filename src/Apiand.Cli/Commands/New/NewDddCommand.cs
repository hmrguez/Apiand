using System.CommandLine;
using Apiand.TemplateEngine.Architectures.DDD;
using Apiand.TemplateEngine.Models;

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

        var apiTypeOption = new Option<string>("--api-type", "API type (FastEndpoints)");
        apiTypeOption.AddAlias("-api");

        var dbTypeOption = new Option<string>("--db-type", "Database type (MongoDB, EF Core)");
        dbTypeOption.AddAlias("-db");

        var applicationOption = new Option<string>("--application", "Application layer type (MediatR)");
        applicationOption.AddAlias("-app");

        var domainOption = new Option<string>("--domain", "Domain layer type (Default)");
        domainOption.AddAlias("-dom");

        AddOption(outputOption);
        AddOption(nameOption);
        AddOption(apiTypeOption);
        AddOption(dbTypeOption);
        AddOption(applicationOption);
        AddOption(domainOption);

        this.SetHandler(HandleDddCommand, outputOption, nameOption, apiTypeOption, dbTypeOption, applicationOption,
            domainOption);
    }

    private void HandleDddCommand(string? output, string name, string api, string? db, string? application,
        string? domain)
    {
        var commandOptions = new CommandOptions
        {
            OutputPath = output ?? "./" + name,
            ProjectName = name,
            ApiType = api,
            DbType = db,
            Application = application,
            Domain = domain
        };

        HandleArchCommand(new DDD(), commandOptions);
    }
}