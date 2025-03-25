using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using Apiand.Cli.Commands.New;

namespace Apiand.Cli.Utils;

public static class Utils
{
    public static void RegisterCommands(RootCommand rootCommand)
    {
        var commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true } && 
                           typeof(Command).IsAssignableFrom(type) &&
                           type != typeof(RootCommand) &&
                           (!typeof(NewCommand).IsAssignableFrom(type) || type == typeof(NewCommand)));

        foreach (var commandType in commandTypes)
        {
            if (Activator.CreateInstance(commandType) is Command command)
            {
                rootCommand.AddCommand(command);
            }
        }
    }
    
    public static void RunDotnetCommand(string workingDirectory, string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        process?.WaitForExit();
    }
}