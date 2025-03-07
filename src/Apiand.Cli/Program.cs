using System.CommandLine;

namespace Apiand.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Create root command
        var rootCommand = new RootCommand("Apiand CLI tool");
        
        // Add commands to root
        Utils.Utils.RegisterCommands(rootCommand);

        // Run the command
        return await rootCommand.InvokeAsync(args);
    }
}