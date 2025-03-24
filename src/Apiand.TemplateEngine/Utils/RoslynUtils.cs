using System.Text.RegularExpressions;
using Apiand.TemplateEngine.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Apiand.TemplateEngine.Utils;

public static class RoslynUtils
{
    public static void RegisterServiceInModule(string project, string serviceName, string serviceNamespace, IMessenger messenger)
    {
        // Find the InfraModule.cs file
        var moduleFiles = Directory.GetFiles(project, "*.cs", SearchOption.AllDirectories)
            .Where(f => File.ReadAllText(f).Contains("IModule") && File.ReadAllText(f).Contains("ConfigureServices"));

        var moduleFilePath = moduleFiles.FirstOrDefault();
        if (string.IsNullOrEmpty(moduleFilePath))
        {
            messenger.WriteErrorMessage("Could not find the IModule implementation in the Infrastructure project.");
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
            messenger.WriteErrorMessage("Could not find the ConfigureServices method in the module.");
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
            messenger.WriteStatusMessage($"Registered service in DI container at {Path.GetFileName(moduleFilePath)}");
        }
        else
        {
            // Try to add before the last closing brace of the method
            Regex methodRegex = new Regex(@"(\s*void\s+ConfigureServices\s*\([^)]*\)\s*\{[^}]*)(\s*\})");
            string newCode = methodRegex.Replace(sourceCode,
                $"$1\n        // Service registrations\n        services.AddScoped<{serviceNamespace}.I{serviceName}Service, {serviceNamespace}.{serviceName}Service>();\n$2");

            File.WriteAllText(moduleFilePath, newCode);
            messenger.WriteStatusMessage($"Registered service in DI container at {Path.GetFileName(moduleFilePath)}");
        }
    }
}