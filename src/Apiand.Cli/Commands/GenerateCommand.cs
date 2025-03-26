using System.CommandLine;
using System.Text.Json;
using Apiand.Cli.Utils;
using Apiand.TemplateEngine;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;

namespace Apiand.Cli.Commands;

public class GenerateCommand : Command
{
    private readonly IMessenger _messenger;
    private readonly TemplateProcessor _processor;

    public GenerateCommand() : base("generate", "Add components to an existing project")
    {
        _processor = new TemplateProcessor();
        _messenger = new ConsoleMessenger();

        // Create the service subcommand
        var serviceCommand = new Command("service", "Add a new service to the project");

        // Add a required argument for the service name
        var serviceNameArgument = new Argument<string>("name", "The name of the service to create");
        serviceCommand.AddArgument(serviceNameArgument);

        var pathOption = new Option<string>("--path", "The path to add the service to");
        pathOption.AddAlias("-p");

        serviceCommand.SetHandler(HandleAddService, serviceNameArgument, pathOption);

        // Add the service subcommand to the add command
        serviceCommand.AddOption(pathOption);
        AddCommand(serviceCommand);

        // In the future, you can add more subcommands like "endpoint", etc.
    }

    private void HandleAddService(string serviceName, string? path)
    {
        var normalizedName = NormalizeServiceName(serviceName);

        _messenger.WriteStatusMessage($"Adding service: {normalizedName}");

        var workingDirectory = string.IsNullOrEmpty(path)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(path);

        // Verify the directory exists
        if (!Directory.Exists(workingDirectory))
        {
            _messenger.WriteErrorMessage($"The specified path '{workingDirectory}' does not exist.");
            return;
        }

        // Find configuration file starting from the working directory
        var configFilePath = FindConfigFile(workingDirectory);
        if (string.IsNullOrEmpty(configFilePath))
        {
            _messenger.WriteErrorMessage(
                "No apiand-config.json found in the specified directory or any parent directories.");
            _messenger.WriteErrorMessage("Please run this command in an Apiand project directory.");
            return;
        }

        _messenger.WriteStatusMessage("Loading project configuration...");

        var config = LoadProjectConfig<TemplateConfiguration>(configFilePath);
        if (config == null)
        {
            _messenger.WriteErrorMessage("Failed to load project configuration.");
            return;
        }

        _messenger.WriteStatusMessage($"Creating service {normalizedName} in project {config.ProjectName}...");

        // Create service data
        var data = new Dictionary<string, string>
        {
            ["name"] = normalizedName,
            ["projectName"] = config.ProjectName
        };

        var projectDir = Path.GetDirectoryName(configFilePath)!;

        var serviceImplementation = ArchitectureTypeFactory.GetCommandImplementations<IGenerateService>(config.ArchName);
        serviceImplementation?.Handle(workingDirectory, projectDir, normalizedName, data, config, _messenger);

        _messenger.WriteSuccessMessage($"Service {normalizedName} added successfully!");
    }

    private string FindConfigFile(string startingDirectory)
    {
        // Start from specified directory and look up for apiand-config.json
        var currentDir = startingDirectory;
        var configFile = Path.Combine(currentDir, "apiand-config.json");

        while (!File.Exists(configFile))
        {
            var parentDir = Directory.GetParent(currentDir)?.FullName;
            if (parentDir == null || parentDir == currentDir)
                return string.Empty;

            currentDir = parentDir;
            configFile = Path.Combine(currentDir, "apiand-config.json");
        }

        return configFile;
    }

    private string NormalizeServiceName(string serviceName)
    {
        // Remove "Service" suffix if present, we'll add it back consistently
        var name = serviceName.EndsWith("Service", StringComparison.OrdinalIgnoreCase)
            ? serviceName.Substring(0, serviceName.Length - 7)
            : serviceName;

        // Capitalize first letter
        if (!string.IsNullOrEmpty(name)) name = char.ToUpper(name[0]) + name.Substring(1);

        return name;
    }

    private TemplateConfiguration? LoadProjectConfig<T>(string configPath) where T : TemplateConfiguration
    {
        try
        {
            var jsonContent = File.ReadAllText(configPath);
            var t = JsonSerializer.Deserialize<T>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (t != null) return t;

            return JsonSerializer.Deserialize<TemplateConfiguration>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (Exception ex)
        {
            _messenger.WriteErrorMessage($"Error loading configuration: {ex.Message}");
            return null!;
        }
    }
}