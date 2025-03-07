using System.CommandLine;

namespace Apiand.Cli.Commands;

public class VersionCommand : Command
{
    public VersionCommand() : base("version", "Displays the version of the tool")
    {
        this.SetHandler(HandleCommand);
    }

    private void HandleCommand()
    {
        Console.WriteLine("Apiand CLI v1.0.0");
    }
}