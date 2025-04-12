using System.CommandLine;
using Apiand.TemplateEngine.Architectures.DDD;
using Apiand.TemplateEngine.Architectures.Microservices;
using Apiand.TemplateEngine.Models;
using Spectre.Console;

namespace Apiand.Cli.Commands.New;

public class NewMicroservicesCommand : NewCommand
{
    public NewMicroservicesCommand() : base("microservices", "Creates a new DDD project")
    {
        // Shared Options
        var outputOption = new Option<string>("--output", "The output directory");
        outputOption.AddAlias("-o");

        var nameOption = new Option<string>("--name", "Project name") { IsRequired = true };
        nameOption.AddAlias("-n");
        
        var interactiveOption = new Option<bool>("--skip-interactive", "Don't use interactive for missing options");
        interactiveOption.AddAlias("-si");

        AddOption(outputOption);
        AddOption(nameOption);
        AddOption(interactiveOption);

        this.SetHandler(HandleCommand, outputOption, nameOption);
    }

    private void HandleCommand(string? output, string name)
    {
        var commandOptions = new CommandOptions
        {
            OutputPath = output ?? "./" + name,
            ProjectName = name,
        };

        var extraOptions = new Dictionary<string, string>();

        HandleArchCommand(new Microservices(), commandOptions, extraOptions);
    }
}