using System.CommandLine;

namespace Apiand.Cli.Commands;

public class GreetCommand : Command
{
    public GreetCommand() : base("greet", "Greets the user")
    {
        var nameOption = new Option<string>("--name", "The name of the person to greet");
        nameOption.AddAlias("-n");
        AddOption(nameOption);

        this.SetHandler(HandleCommand, nameOption);
    }

    private void HandleCommand(string name)
    {
        Console.WriteLine($"Hello {name}!");
    }
}