using System.CommandLine;

namespace Apiand.Cli.Commands;

public class CreateNewCommand: Command
{
    public CreateNewCommand() : base("create", "Creates a new default template")
    {
        this.SetHandler(HandleCommand);
    }
    
    private void HandleCommand()
    {
        
    }
}