using Xunit.Abstractions;

namespace CliTests;

public class ApiandProjectTests : ApiandTestBase
{
    private const string DefaultProjectName = "TestProject";

    public ApiandProjectTests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData("ddd", "--presentation FastEndpoints --application MediatR --infrastructure EFCore")]
    [InlineData("ddd", "--infrastructure MongoDB")]
    [InlineData("ddd", "")] // Default options
    [InlineData("single-layer", "")]
    [InlineData("microservices", "")]
    public void Create_Project_Should_Build(string architecture, string options)
    {
        // Arrange
        var projectDir = GetProjectPath(DefaultProjectName);

        // Act
        var createResult = RunCommand("apiand",
            $"new {architecture} --output {projectDir} --name {DefaultProjectName} --skip-interactive {options}"
                .Trim());

        // Assert
        Assert.Equal(0, createResult);
        Assert.True(VerifyBuild(projectDir));
    }

    [Theory]
    [InlineData("ddd", "service Orders.User")]
    [InlineData("ddd", "endpoint GetUsers --http-method Get")]
    [InlineData("ddd",
        "entity Orders.Customer --attributes \"name:string;email:string;status:enum[active,inactive]\"")]
    [InlineData("single-layer", "service Products.Catalog")]
    [InlineData("single-layer", "endpoint CreateProduct --http-method Post")]
    [InlineData("microservices", "project Inventory")]
    public void Generate_Components_Should_Build(string architecture, string generateCommand)
    {
        // Arrange
        var projectDir = GetProjectPath(DefaultProjectName);

        // Act
        var createResult = RunCommand("apiand",
            $"new {architecture} --output {projectDir} --name {DefaultProjectName} --skip-interactive");
        Assert.Equal(0, createResult);

        var generateResult = RunCommand("apiand", $"generate {generateCommand} -o {projectDir}");

        // Assert
        Assert.Equal(0, generateResult);
        Assert.True(VerifyBuild(projectDir));
    }

    [Theory]
    [InlineData("ddd",
        new[]
        {
            "service Orders.User", "endpoint GetUsers --http-method Get",
            "entity Orders.Customer --attributes \"name:string\""
        })]
    [InlineData("single-layer",
        new[] { "service Products.Catalog", "endpoint CreateProduct --http-method Post", "entity Products.Item" })]
    public void Multiple_Generations_Should_Build(string architecture, string[] generateCommands)
    {
        // Arrange
        var projectDir = GetProjectPath(DefaultProjectName);

        // Act
        var createResult = RunCommand("apiand",
            $"new {architecture} --output {projectDir} --name {DefaultProjectName} --skip-interactive");
        Assert.Equal(0, createResult);

        // Generate multiple components
        foreach (var cmd in generateCommands)
        {
            var generateResult = RunCommand("apiand", $"generate {cmd} -o {projectDir}");
            Assert.Equal(0, generateResult);
        }

        // Assert
        Assert.True(VerifyBuild(projectDir));
    }

    [Theory]
    [InlineData("microservices", "project Inventory", "service Orders.ShippingDetails")]
    [InlineData("microservices", "project UserManagement", "endpoint GetUsers --http-method Get")]
    public void Microservices_Project_With_Services_Should_Build(string architecture, string projectCmd,
        string serviceCmd)
    {
        // Arrange
        var projectDir = GetProjectPath(DefaultProjectName);
        var projectPath = Path.Combine(projectDir, projectCmd.Split(' ')[1]);

        // Act
        var createResult = RunCommand("apiand",
            $"new {architecture} --output {projectDir} --name {DefaultProjectName} --skip-interactive");
        Assert.Equal(0, createResult);

        var projectResult = RunCommand("apiand", $"generate {projectCmd} -o {projectDir}");
        Assert.Equal(0, projectResult);
        
        var serviceResult = RunCommand("apiand", $"generate {serviceCmd} -o {Path.Combine(projectDir, projectPath)}");
        Assert.Equal(0, serviceResult);

        // Assert
        Assert.True(VerifyBuild(projectDir));
    }
}