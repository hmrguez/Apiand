using System.CommandLine;
using System.IO;
using System.Text.Json;
using Apiand.TemplateEngine;
using Apiand.TemplateEngine.Architectures.DDD;
using Apiand.TemplateEngine.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Apiand.Cli.Commands;

public class AddCommand : Command
{
    private readonly TemplateProcessor _processor;

    public AddCommand() : base("add", "Add components to an existing project")
    {
        _processor = new TemplateProcessor();

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
        // Normalize service name
        string normalizedName = NormalizeServiceName(serviceName);
    
        WriteStatusMessage($"Adding service: {normalizedName}");
    
        // Use the specified path as working directory if provided
        string workingDirectory = string.IsNullOrEmpty(path) 
            ? Directory.GetCurrentDirectory() 
            : Path.GetFullPath(path);
    
        // Verify the directory exists
        if (!Directory.Exists(workingDirectory))
        {
            WriteErrorMessage($"The specified path '{workingDirectory}' does not exist.");
            return;
        }
    
        // Find configuration file starting from the working directory
        string configFilePath = FindConfigFile(workingDirectory);
        if (string.IsNullOrEmpty(configFilePath))
        {
            WriteErrorMessage("No apiand-config.json found in the specified directory or any parent directories.");
            WriteErrorMessage("Please run this command in an Apiand project directory.");
            return;
        }
    
        WriteStatusMessage("Loading project configuration...");
        var config = LoadProjectConfig<DddTemplateConfiguration>(configFilePath);
        if (config == null)
        {
            WriteErrorMessage("Failed to load project configuration.");
            return;
        }
    
        string projectDir = Path.GetDirectoryName(configFilePath);
    
        // Example placeholder implementation
        WriteStatusMessage($"Creating service {normalizedName} in project {config.ProjectName}...");
    
        // Create service data
        var data = new Dictionary<string, string>
        {
            ["name"] = normalizedName,
            ["projectName"] = config.ProjectName
        };
    
        // TODO: Implement logic to:
        // Check if we are working with DDD architecture
        bool isDddArchitecture = config.GetType().Name == nameof(DddTemplateConfiguration);
        
        // Parse the service name for subdirectories (e.g., "UseCases.User" -> ["UseCases", "User"])
        string[] nameParts = normalizedName.Split('.');
        string serviceClassName = nameParts[^1]; // Last part is the actual service name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));
        
        // Find the appropriate projects for interface and implementation
        
        string applicationProject = null;
        string infrastructureProject = null;
        
        if (isDddArchitecture)
        {
            // For DDD architecture, find Application and Infrastructure projects
            var projectFiles = Directory.GetFiles(projectDir, "*.csproj", SearchOption.AllDirectories);
            
            foreach (var projectFile in projectFiles)
            {
                string projectFileName = Path.GetFileNameWithoutExtension(projectFile);
                if (projectFileName.EndsWith("Application"))
                {
                    applicationProject = Path.GetDirectoryName(projectFile);
                }
                else if (projectFileName.EndsWith("Infrastructure"))
                {
                    infrastructureProject = Path.GetDirectoryName(projectFile);
                }
            }
        
            if (applicationProject == null || infrastructureProject == null)
            {
                WriteErrorMessage("Could not find required Application and Infrastructure projects in DDD architecture.");
                return;
            }
        }
        else
        {
            // For non-DDD, use the project root
            applicationProject = infrastructureProject = projectDir;
        }
        
        // Create interface and implementation templates
        // Create interface and implementation templates
        string interfaceContent = 
            $$"""
            namespace {{config.ProjectName}}.Application.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
                    
            public interface I{{serviceName}}Service
            {
                // TODO: Add service methods
            }
            """;
        
        string implementationContent = 
            $$"""
             using {{config.ProjectName}}.Application.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
             
             namespace {{config.ProjectName}}.Infrastructure.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
             
             public class {{serviceName}}Service : I{{serviceName}}Service
             {
                 // TODO: Implement service methods
             }
             """;
        
        // Create directories and files
        string interfaceDir = Path.Combine(applicationProject, "Services", subDirPath);
        string implementationDir = Path.Combine(infrastructureProject, "Services", subDirPath);
        
        Directory.CreateDirectory(interfaceDir);
        Directory.CreateDirectory(implementationDir);
        
        string interfacePath = Path.Combine(interfaceDir, $"I{serviceClassName}Service.cs");
        string implementationPath = Path.Combine(implementationDir, $"{serviceClassName}Service.cs");
        
        // Process the content using the template engine if needed
        var interfaceTemplateData = new Dictionary<string, string>(data);
        var implementationTemplateData = new Dictionary<string, string>(data);
        
        interfaceTemplateData["serviceNamespace"] = $"{config.ProjectName}.Application.Services{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";
        implementationTemplateData["serviceNamespace"] = $"{config.ProjectName}.Infrastructure.Services{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";
        interfaceTemplateData["interfaceNamespace"] = interfaceTemplateData["serviceNamespace"];
        
        // Write files
        File.WriteAllText(interfacePath, interfaceContent);
        File.WriteAllText(implementationPath, implementationContent);
        
        WriteStatusMessage($"Created interface at {Path.GetRelativePath(projectDir, interfacePath)}");
        WriteStatusMessage($"Created implementation at {Path.GetRelativePath(projectDir, implementationPath)}");
    
        // After creating interface and implementation files
        if (isDddArchitecture)
        {
            // For DDD architecture, register the service in DI
            string serviceNamespace = $"{config.ProjectName}.Application.Services{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";
            RegisterServiceInDI(infrastructureProject, serviceClassName, serviceNamespace);
        }
        
        WriteSuccessMessage($"Service {normalizedName} added successfully!");
    }
    
    private string FindConfigFile(string startingDirectory)
    {
        // Start from specified directory and look up for apiand-config.json
        string currentDir = startingDirectory;
        string configFile = Path.Combine(currentDir, "apiand-config.json");
    
        while (!File.Exists(configFile))
        {
            string parentDir = Directory.GetParent(currentDir)?.FullName;
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
        string name = serviceName.EndsWith("Service", StringComparison.OrdinalIgnoreCase)
            ? serviceName.Substring(0, serviceName.Length - 7)
            : serviceName;

        // Capitalize first letter
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
            string jsonContent = File.ReadAllText(configPath);
            var t = JsonSerializer.Deserialize<T>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (t != null) return t;
            
            
            return JsonSerializer.Deserialize<TemplateConfiguration>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        }
        catch (Exception ex)
        {
            WriteErrorMessage($"Error loading configuration: {ex.Message}");
            return null!;
        }
    }
    
    private void RegisterServiceInDI(string infrastructureProject, string serviceName, string serviceNamespace)
    {
        // Find the InfraModule.cs file
        var moduleFiles = Directory.GetFiles(infrastructureProject, "*.cs", SearchOption.AllDirectories)
            .Where(f => File.ReadAllText(f).Contains("IModule") && File.ReadAllText(f).Contains("ConfigureServices"));
        
        string moduleFilePath = moduleFiles.FirstOrDefault();
        if (string.IsNullOrEmpty(moduleFilePath))
        {
            WriteErrorMessage("Could not find the IModule implementation in the Infrastructure project.");
            return;
        }
    
        // Read the file content
        string sourceCode = File.ReadAllText(moduleFilePath);
        
        // Parse the source code
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = syntaxTree.GetRoot();
        
        // Find the ConfigureServices method
        var methodDeclaration = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == "ConfigureServices");
        
        if (methodDeclaration == null)
        {
            WriteErrorMessage("Could not find the ConfigureServices method in the module.");
            return;
        }
        
        // Check if there's a comment marker
        string markerComment = "// Add your services here (DO NOT REMOVE THIS LINE)";
        bool hasMarker = methodDeclaration.ToString().Contains(markerComment);
        
        if (hasMarker)
        {
            // Insert the service registration after the marker
            string newCode = sourceCode.Replace(
                markerComment,
                $"{markerComment}\n        services.AddScoped<{serviceNamespace}.I{serviceName}Service, {serviceNamespace}.{serviceName}Service>();"
            );
            
            File.WriteAllText(moduleFilePath, newCode);
            WriteStatusMessage($"Registered service in DI container at {Path.GetFileName(moduleFilePath)}");
        }
        else
        {
            // Try to add before the last closing brace of the method
            Regex methodRegex = new Regex(@"(\s*void\s+ConfigureServices\s*\([^)]*\)\s*\{[^}]*)(\s*\})");
            string newCode = methodRegex.Replace(sourceCode, 
                $"$1\n        // Service registrations\n        services.AddScoped<{serviceNamespace}.I{serviceName}Service, {serviceNamespace}.{serviceName}Service>();\n$2");
            
            File.WriteAllText(moduleFilePath, newCode);
            WriteStatusMessage($"Registered service in DI container at {Path.GetFileName(moduleFilePath)}");
        }
    }

    private void WriteStatusMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("→ ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void WriteSuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("✓ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void WriteErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("✗ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}