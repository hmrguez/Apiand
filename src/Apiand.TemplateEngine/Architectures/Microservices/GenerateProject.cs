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
        var projectScaffoldPath = Path.Combine(TemplateUtils.TemplatePath, ArchName, "Service");

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