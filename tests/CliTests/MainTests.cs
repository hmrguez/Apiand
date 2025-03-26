using System.Diagnostics;

namespace CliTests;

public class AddServiceCommandTests : IDisposable
{
    private readonly string _projectName = "TestApi";
    private readonly string _tempDirectory;

    public AddServiceCommandTests()
    {
        // Create temporary directory for testing
        _tempDirectory = Path.Combine(Path.GetTempPath(), "apiand_tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        // Clean up test directory after test run
        if (Directory.Exists(_tempDirectory)) Directory.Delete(_tempDirectory, true);
    }

    [Fact]
    public async Task Should_Build_Successfully()
    {
        // Arrange
        var projectPath = Path.Combine(_tempDirectory, _projectName);

        // Act
        // Step 1: Create a new DDD project
        var createResult = await ExecuteCliCommand("new",
            $"ddd --output {projectPath} --name {_projectName} --api-type Fast-Endpoints --db-type Mongo-D-B");
        Assert.Equal(0, createResult);

        // Step 2: Add a User service
        var addServiceResult = await ExecuteCliCommand("generate", $"service User -p {projectPath}");
        Assert.Equal(0, addServiceResult);

        // Assert
        // Step 3: Verify project builds
        var buildResult = await ExecuteDotnetCommand("build", projectPath);
        Assert.Equal(0, buildResult);
    }

    private async Task<int> ExecuteCliCommand(string command, string arguments)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "apiand",
            Arguments = $"{command} {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(processInfo);
        await process.WaitForExitAsync();

        return process.ExitCode;
    }

    private async Task<int> ExecuteDotnetCommand(string command, string workingDirectory)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = command,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(processInfo);
        await process.WaitForExitAsync();

        return process.ExitCode;
    }
}