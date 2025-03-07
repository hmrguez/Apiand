using System.CommandLine;
using System.Reflection;

namespace Apiand.Cli.Utils;

public static class Utils
{
    public static void RegisterCommands(RootCommand rootCommand)
    {
        var commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsClass: true } && 
                           typeof(Command).IsAssignableFrom(type) && 
                           type != typeof(RootCommand));

        foreach (var commandType in commandTypes)
        {
            if (Activator.CreateInstance(commandType) is Command command)
            {
                rootCommand.AddCommand(command);
            }
        }
    }
}