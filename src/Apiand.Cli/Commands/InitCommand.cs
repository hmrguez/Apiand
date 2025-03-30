using System.CommandLine;
using System.IO;
using System.Text.Json;
using Apiand.Cli.Utils;
using Apiand.TemplateEngine.Models;

namespace Apiand.Cli.Commands;

public class InitCommand : Command
{
    private readonly IMessenger _messenger = new ConsoleMessenger();

    public InitCommand() : base("init", "Initialize a new Apiand project configuration")
    {
        var projectNameOption = new Option<string?>(
            "--name",
            "Name of the project (defaults to current directory name)");

        var outputPathOption = new Option<string>(
            "--output",
            getDefaultValue: () => ".",
            description: "Path where the configuration file should be written");

        AddOption(projectNameOption);
        AddOption(outputPathOption);

        this.SetHandler(HandleCommand, projectNameOption, outputPathOption);
    }

    private void HandleCommand(string? projectName, string outputPath)
    {
        _messenger.WriteStatusMessage("Initializing Apiand project configuration...");
        
        // Get the current directory name if project name is not provided
        if (string.IsNullOrWhiteSpace(projectName))
        {
            projectName = new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
            _messenger.WriteStatusMessage($"Using current directory name as project name: {projectName}");
        }

        // Create the template configuration
        var configuration = new TemplateConfiguration
        {
            OutputPath = outputPath,
            ProjectName = projectName,
            ArchName = "Standalone"
        };

        // Make sure the directory exists
        Directory.CreateDirectory(outputPath);

        // Serialize the configuration to JSON
        var fileName = Path.Combine(outputPath, "apiand.config.json");
        var jsonString = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, jsonString);

        _messenger.WriteSuccessMessage($"Configuration file created at: {fileName}");
    }
}
