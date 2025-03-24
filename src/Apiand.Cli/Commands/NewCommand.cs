using System.CommandLine;
using System.Diagnostics;
using Apiand.TemplateEngine;
using Apiand.TemplateEngine.Models;

namespace Apiand.Cli.Commands;

public class NewCommand : Command
{
    private readonly TemplateProcessor _processor;

    public NewCommand() : base("new", "Creates a new project from a template")
    {
        _processor = new TemplateProcessor();

        var outputOption = new Option<string>("--output", "The output directory") { IsRequired = true };
        outputOption.AddAlias("-o");

        var nameOption = new Option<string>("--name", "Project name");
        nameOption.AddAlias("-n");

        // Architecture options
        var architectureOption = new Option<string>("--architecture",
            "Architecture type (single-layer, multi-layer, microservices)");
        architectureOption.AddAlias("-a");

        // API options
        var apiTypeOption = new Option<string>("--api-type", "API type (rest, graphql, grpc)");
        apiTypeOption.AddAlias("-api");

        // Database options
        var dbTypeOption = new Option<string>("--db-type", "Database type (mongodb, sqlserver, postgres, none)");
        dbTypeOption.AddAlias("-db");
        
        // Application and domain options
        var applicationOption = new Option<string>("--application", "Application layer type (mvc, webapi, none)");
        applicationOption.AddAlias("-app");
        
        var domainOption = new Option<string>("--domain", "Domain layer type (entityframework, dapper, none)");
        domainOption.AddAlias("-dom");

        AddOption(outputOption);
        AddOption(nameOption);
        AddOption(architectureOption);
        AddOption(apiTypeOption);
        AddOption(dbTypeOption);
        AddOption(applicationOption);
        AddOption(domainOption);

        this.SetHandler(HandleCommand, outputOption, nameOption, architectureOption, apiTypeOption,
            dbTypeOption, applicationOption, domainOption);
    }

    private void HandleCommand(string output, string? name, string architecture, string apiType,
        string dbType, string? application, string? domain)
    {
        WriteStatusMessage("Validating configuration...");
    
        var arch = ArchitectureTypeFactory.Create(architecture);
        arch.LoadVariants(TemplateUtils.TemplatePath);
    
        var commandOptions = new CommandOptions()
        {
            OutputPath = output,
            ProjectName = name,
            Architecture = architecture,
            ApiType = apiType,
            DbType = dbType,
            Application = application,
            Domain = domain
        };
    
        var config = arch.BuildConfig(commandOptions);
        var validation = arch.Validate(config);
        if (!validation.IsValid)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid configuration:");
            foreach (var error in validation.Errors)
            {
                Console.WriteLine($"- {error}");
            }
    
            Console.ResetColor();
            return;
        }
    
        WriteStatusMessage("Fetching templates...");
        var templatePaths = arch.Resolve(config);
        if (templatePaths.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No matching templates found for the given configuration.");
            Console.ResetColor();
            return;
        }
    
        WriteStatusMessage("Generating project...");
        var data = new Dictionary<string, string>
        {
            ["name"] = config.ProjectName,
        };
    
        _processor.CreateFromTemplateVariants(templatePaths, output, data);
        
        // Create a new empty solution
        WriteStatusMessage("Creating solution file...");
        string solutionName = string.IsNullOrEmpty(config.ProjectName) ? Path.GetFileName(output) : config.ProjectName;
        RunDotnetCommand(output, $"new sln -n {solutionName}");
        
        // Find all .csproj files and add them to the solution
        WriteStatusMessage("Adding projects to solution...");
        var projectFiles = Directory.GetFiles(output, "*.csproj", SearchOption.AllDirectories);
        foreach (var projectFile in projectFiles)
        {
            // Get the relative path from the solution directory to the project file
            string relativePath = Path.GetRelativePath(output, projectFile);
            RunDotnetCommand(output, $"sln add {relativePath}");
        }
        
        // In the HandleCommand method, after adding projects to the solution:
        WriteStatusMessage("Setting up project references...");
        arch.ExecutePostCreationCommands(output, solutionName);
    
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\u2728 Project created in ");
        Console.Write(output);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine();
        Console.ResetColor();
    }
    
    private void RunDotnetCommand(string workingDirectory, string arguments)
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
        
        // You might want to handle or log process output
        // var output = process?.StandardOutput.ReadToEnd();
        // var error = process?.StandardError.ReadToEnd();
    }
    
    private void WriteStatusMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("✓ ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}