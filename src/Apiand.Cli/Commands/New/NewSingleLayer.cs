using System.CommandLine;
using Apiand.TemplateEngine.Architectures.DDD;
using Apiand.TemplateEngine.Architectures.SingleLayer;
using Apiand.TemplateEngine.Models;

namespace Apiand.Cli.Commands.New;

public class NewSingleLayer : NewCommand
{
    public NewSingleLayer() : base("single-layer", "Creates a new single layer architecture")
    {
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
            ProjectName = name
        };

        HandleArchCommand(new SingleLayer(), commandOptions, new Dictionary<string, string>());
    }
}