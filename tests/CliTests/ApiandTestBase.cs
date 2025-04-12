using System.Diagnostics;
using Xunit.Abstractions;

namespace CliTests;

public abstract class ApiandTestBase : IDisposable
{
    protected readonly string _baseOutputDir;
    protected readonly ITestOutputHelper _output;
    protected readonly string _toolName = "Apiand.Cli";

    public ApiandTestBase(ITestOutputHelper output)
    {
        _output = output;
        _baseOutputDir = Path.Combine(Path.GetTempPath(), $"ApiandTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_baseOutputDir);

        // Install the tool: TODO: This is disabled because it takes too long and should be enabled if decide to add to cicd
        // RunCommand("dotnet", "pack ./src/Apiand.Cli -c Release -o ./nupkg");
        // RunCommand("dotnet", $"tool uninstall --global {_toolName}");
        // RunCommand("dotnet", $"tool install --global --add-source ../nupkg {_toolName}");
    }

    public void Dispose()
    {
        try
        {
            // Clean up resources
            if (Directory.Exists(_baseOutputDir)) Directory.Delete(_baseOutputDir, true);
            // RunCommand("dotnet", $"tool uninstall --global {_toolName}");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Clean up error: {ex.Message}");
        }
    }

    protected string GetProjectPath(string projectName)
    {
        return Path.Combine(_baseOutputDir, projectName);
    }

    protected int RunCommand(string command, string arguments, string workingDirectory = null)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory()
            }
        };

        _output.WriteLine($"Running: {command} {arguments}");

        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        _output.WriteLine($"Output: {output}");
        if (!string.IsNullOrEmpty(error)) _output.WriteLine($"Error: {error}");

        return process.ExitCode;
    }

    protected bool VerifyBuild(string projectDir)
    {
        var exitCode = RunCommand("dotnet", "build", projectDir);
        return exitCode == 0;
    }
}