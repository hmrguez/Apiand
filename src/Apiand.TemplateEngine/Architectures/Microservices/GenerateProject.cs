using Apiand.Extensions.Models;
using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;

namespace Apiand.TemplateEngine.Architectures.Microservices;

public class GenerateProject : IGenerateProject
{
    public string ArchName { get; set; } = "Microservices";

    public Result Handle(string workingDirectory, string projectDirectory, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var useGraphQl = extraData["graphql"] == "true";
        var projectScaffoldPath = Path.Combine(TemplateUtils.TemplatePath, ArchName, useGraphQl ? "GraphQL" : "Service");

        var data = new Dictionary<string, string>(extraData)
        {
            ["name"] = configuration.ProjectName,
            ["service"] = argument
        };

        // Create output path
        string outputPath = Path.Combine(projectDirectory, $"{data["name"]}.{data["service"]}");

        try
        {
            // Process the template
            var processor = new TemplateProcessor();
            var metadata = new TemplateMetadata(); // Default metadata
            
            Directory.CreateDirectory(outputPath);
            processor.CopyAndProcessDirectory(projectScaffoldPath, outputPath, data, metadata);

            // Add the project to the solution
            string? solutionPath = Directory.GetFiles(projectDirectory, "*.sln").FirstOrDefault();
            if (solutionPath == null) return Result.Fail(TemplatingErrors.SolutionNotFound);
            
            string? projectFile = Directory.GetFiles(outputPath, "*.csproj").FirstOrDefault();
            if (projectFile == null) return Result.Fail(TemplatingErrors.ProjectNotFound);
            
            var appHostProjectPath = Path.Combine(projectDirectory, $"{data["name"]}.AppHost", $"{data["name"]}.Apphost.csproj");
            if (File.Exists(appHostProjectPath))
            {
                messenger.WriteStatusMessage("Adding to the Aspire dashboard");
                var addReferenceProcess = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"add \"{appHostProjectPath}\" reference \"{projectFile}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                

                addReferenceProcess.Start();
                addReferenceProcess.WaitForExit();
                
                // Also update the Program.cs in the Apphost to include the new service
                var programPath = Path.Combine(projectDirectory, $"{data["name"]}.AppHost", "Program.cs");
                if (File.Exists(programPath))
                {
                    var content = File.ReadAllText(programPath);
                    var projectTypeName = $"{data["name"]}_{data["service"]}";
                    var serviceName = data["service"].ToLower();

                    // Only add if not already in the file
                    if (!content.Contains(projectTypeName))
                    {
                        var lastBuildIndex = content.LastIndexOf("builder.Build()");
                        if (lastBuildIndex > 0)
                        {
                            var insertPosition = content.LastIndexOf(';', lastBuildIndex);
                            if (insertPosition > 0)
                            {
                                var newService = 
                                    $"""
                                    
                                    builder.AddProject<{projectTypeName}>("{serviceName}")
                                        .WithReference(mongodb)
                                        .WithReference(rabbitmq)
                                        .WaitFor(mongodb)
                                        .WaitFor(rabbitmq);
                                        
                                    """;
                                
                                content = content.Insert(insertPosition + 2, newService);
                                File.WriteAllText(programPath, content);
                            }
                        }
                    }
                    else
                    {
                        messenger.WriteStatusMessage($"Service {serviceName} already registered in Apphost Program.cs.");
                    }
                }
            }
            else
            {
                messenger.WriteWarningMessage("No .NET Aspire AppHost found, skipping adding to the Aspire dashboard");
            }


            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"sln \"{solutionPath}\" add \"{projectFile}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                messenger.WriteStatusMessage($"Added {Path.GetFileName(projectFile)} to solution.");
            }
            else
            {
                return Result.Fail(new Error("unknown", process.StandardError.ReadToEnd()));
            }

            return Result.Succeed();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("unknown", ex.Message));
        }
    }
}