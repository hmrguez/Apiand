using System.CommandLine;
using System.Text;
using System.Text.Json;
using Apiand.Cli.Utils;
using Apiand.TemplateEngine;
using Apiand.TemplateEngine.Models;

namespace Apiand.Cli.Commands.New;

public class NewCommand : Command
{
    protected readonly TemplateProcessor Processor = new();
    protected readonly IMessenger Messenger = new ConsoleMessenger();

    // This constructor is used by the subcommands
    protected NewCommand(string arch, string description) : base(arch, description)
    {
    }
    
    // This constructor is used by the root command
    public NewCommand() : base("new", $"Creates a new project from a template.")
    {
        RegisterSubcommands();
    }

    private void RegisterSubcommands()
    {
        AddCommand(new NewDddCommand());
        AddCommand(new NewSingleLayer());
    }

    protected void HandleArchCommand(ArchitectureType arch, CommandOptions commandOptions)
    {
        Messenger.WriteStatusMessage("Validating configuration...");

        arch.LoadVariants(TemplateUtils.TemplatePath);

        var config = arch.BuildConfig(commandOptions);
        var validation = arch.Validate(config);
        if (!validation.IsValid)
        {
            StringBuilder sb = new();
            sb.AppendLine("Invalid configuration:");
            foreach (var error in validation.Errors)
            {
                sb.AppendLine($"- {error}");
            }

            Messenger.WriteErrorMessage(sb.ToString());
            return;
        }

        Messenger.WriteStatusMessage("Fetching templates...");
        var templatePaths = arch.Resolve(config);
        if (templatePaths.Count == 0)
        {
            Messenger.WriteErrorMessage("No matching templates found for the given configuration.");
            return;
        }

        Messenger.WriteStatusMessage("Generating project...");
        var data = new Dictionary<string, string>
        {
            ["name"] = config.ProjectName,
        };

        Processor.CreateFromTemplateVariants(templatePaths, commandOptions.OutputPath, data);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };

        var json = JsonSerializer.Serialize(config, config.GetType(), jsonOptions);
        File.WriteAllText(Path.Combine(commandOptions.OutputPath, "apiand.config.json"), json);


        // Create a new empty solution
        Messenger.WriteStatusMessage("Creating solution file...");
        string solutionName = string.IsNullOrEmpty(config.ProjectName) ? Path.GetFileName(commandOptions.OutputPath) : config.ProjectName;
        Utils.Utils.RunDotnetCommand(commandOptions.OutputPath, $"new sln -n {solutionName}");

        // Find all .csproj files and add them to the solution
        Messenger.WriteStatusMessage("Adding projects to solution...");
        var projectFiles = Directory.GetFiles(commandOptions.OutputPath, "*.csproj", SearchOption.AllDirectories);
        foreach (var projectFile in projectFiles)
        {
            // Get the relative path from the solution directory to the project file
            string relativePath = Path.GetRelativePath(commandOptions.OutputPath, projectFile);
            Utils.Utils.RunDotnetCommand(commandOptions.OutputPath, $"sln add {relativePath}");
        }

        // In the HandleCommand method, after adding projects to the solution:
        Messenger.WriteStatusMessage("Setting up project references...");
        arch.ExecutePostCreationCommands(commandOptions.OutputPath, solutionName);
        
        Messenger.WriteSuccessMessage("Project created in " + commandOptions.OutputPath);
    }
}