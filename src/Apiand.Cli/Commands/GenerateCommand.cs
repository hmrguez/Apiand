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

        RegisterComponentGenerator("service", "Add a new service to the project", typeof(IGenerateService));
        RegisterComponentGenerator("endpoint", "Add a new endpoint to the project", typeof(IGenerateEndpoint));
        RegisterComponentGenerator("entity", "Add a new entity to the project", typeof(IGenerateEntity));
        RegisterComponentGenerator("project", "Add a new project to the microservices solution",
            typeof(IGenerateProject));
    }

    private void RegisterComponentGenerator(string commandName, string description, Type implementationType)
    {
        var command = new Command(commandName, description);

        // Add a required argument for the component name
        var nameArgument = new Argument<string>("name", $"The name of the {commandName} to create");
        command.AddArgument(nameArgument);

        var outputOption = new Option<string>("--output", $"The path to add the {commandName} to");
        outputOption.AddAlias("-o");
        command.AddOption(outputOption);

        // Add HTTP method option only for endpoint command
        Option<string>? httpMethodOption = null;
        if (commandName == "endpoint")
        {
            httpMethodOption = new Option<string>(
                "--http-method",
                () => "Get",
                "The HTTP method for the endpoint (Get, Post, Put, Delete, Patch)");
            httpMethodOption.AddAlias("-m");
            command.AddOption(httpMethodOption);

            command.SetHandler(
                (name, path, httpMethod) =>
                {
                    var data = new Dictionary<string, string>
                    {
                        ["http-method"] = httpMethod,
                    };
                    HandleGenerateComponent(name, path, commandName, implementationType, data);
                },
                nameArgument, outputOption, httpMethodOption);
        }
        else if (commandName == "entity")
        {
            var attributesOption = new Option<string>(
                "--attributes",
                "Entity attributes in format 'name:type,email:string,status:enum[value1,value2]'");
            attributesOption.AddAlias("-a");
            command.AddOption(attributesOption);

            command.SetHandler(
                (name, path, attributes) =>
                {
                    var data = new Dictionary<string, string>
                    {
                        ["attributes"] = attributes,
                    };
                    HandleGenerateComponent(name, path, commandName, implementationType, data);
                },
                nameArgument, outputOption, attributesOption);
        }
        else
        {
            command.SetHandler(
                (name, path) =>
                {
                    var data = new Dictionary<string, string>
                    {
                    };
                    HandleGenerateComponent(name, path, commandName, implementationType, data);
                },
                nameArgument, outputOption);
        }

        AddCommand(command);
    }

    private void HandleGenerateComponent(string componentName, string? path, string componentType,
        Type implementationType, Dictionary<string, string>? extraData = null)
    {
        var normalizedName = NormalizeName(componentName, componentType);
        _messenger.WriteStatusMessage($"Adding {componentType}: {normalizedName}");

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


        TemplateConfiguration? config = null;
        if (!File.Exists(configFilePath))
        {
            _messenger.WriteWarningMessage("Failed to load project configuration. Defaulting to standalone");
            config = TemplateConfiguration.Default(workingDirectory);
        }
        
        if (config == null)
        {
            config = LoadProjectConfig<TemplateConfiguration>(configFilePath);
            if (config == null)
            {
                _messenger.WriteWarningMessage("Failed to load project configuration. Defaulting to standalone");
                config = TemplateConfiguration.Default(workingDirectory);
            }
        }

        _messenger.WriteStatusMessage($"Creating {componentType} {normalizedName} in project {config.ProjectName}...");

        // Create component data
        var data = extraData ?? new Dictionary<string, string>();
        data["name"] = normalizedName;
        data["projectName"] = config.ProjectName;

        var projectDir = Path.GetDirectoryName(configFilePath)!;

        // Get the appropriate implementation based on component type and architecture
        var implementation = ArchitectureTypeFactory.GetCommandImplementation(implementationType, config.ArchName);
        if (implementation == null)
        {
            _messenger.WriteErrorMessage(
                $"No implementation found for generate {componentType} in architecture {config.ArchName}, defaulting to standalone");
            return;
        }

        var result = implementation.Handle(workingDirectory, projectDir, normalizedName, data, config, _messenger);
        if (result.IsSuccess)
            _messenger.WriteSuccessMessage(
                $"{char.ToUpper(componentType[0])}{componentType.Substring(1)} {normalizedName} added successfully!");
        else
            _messenger.WriteErrorMessage(result.Error!.Description);
    }

    private string FindConfigFile(string startingDirectory)
    {
        // Start from specified directory and look up for apiand.config.json
        var currentDir = startingDirectory;
        var configFile = Path.Combine(currentDir, "apiand.config.json");
        return configFile;
    }

    private string NormalizeName(string name, string componentType)
    {
        // Remove type suffix if present (e.g., "UserService" -> "User" when component type is "service")
        var typeSuffix = componentType.Substring(0, 1).ToUpper() + componentType.Substring(1);
        if (name.EndsWith(typeSuffix, StringComparison.OrdinalIgnoreCase))
        {
            name = name.Substring(0, name.Length - typeSuffix.Length);
        }

        // Handle dot-separated format by capitalizing each segment
        if (name.Contains('.'))
        {
            var segments = name.Split('.');
            for (int i = 0; i < segments.Length; i++)
            {
                if (!string.IsNullOrEmpty(segments[i]))
                {
                    segments[i] = char.ToUpper(segments[i][0]) + segments[i].Substring(1);
                }
            }

            return string.Join(".", segments);
        }

        // Capitalize first letter for simple names
        if (!string.IsNullOrEmpty(name))
        {
            name = char.ToUpper(name[0]) + name.Substring(1);
        }

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