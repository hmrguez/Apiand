using System.CommandLine;
using Apiand.TemplateEngine.Ai;
using Microsoft.Extensions.DependencyInjection;

namespace Apiand.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Create root command
        var rootCommand = new RootCommand("Apiand CLI tool");

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<TemplateEngine.TemplateEngine>(); // Example registration
        serviceCollection.AddTransient<TemplateEngine.TemplateProcessor>(); // Example registration

        
        // Add commands to root
        Utils.Utils.RegisterCommands(rootCommand);
        
        // Run the command
        return await rootCommand.InvokeAsync(args);
    }
}