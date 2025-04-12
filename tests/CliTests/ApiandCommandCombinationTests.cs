using Xunit.Abstractions;

namespace CliTests
{
    public class ApiandCommandCombinationTests : ApiandTestBase
    {
        private const string DefaultProjectName = "ComboTest";

        public ApiandCommandCombinationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("ddd", new[]
        {
            "service Orders.User",
            "service Products.Catalog",
            "endpoint GetUsers --http-method Get ",
            "endpoint CreateProduct --http-method Post",
            "entity Orders.Customer --attributes \"name:string;email:string\" ",
            "entity Products.Item --attributes \"sku:string;price:decimal\""
        })]
        [InlineData("single-layer", new[]
        {
            "service Orders.Shipment",
            "service Products.Inventory",
            "endpoint GetShipment --http-method Get",
            "endpoint UpdateInventory --http-method Put",
            "entity Orders.ShipmentDetails",
            "entity Products.Stock"
        })]
        public void Complex_Project_Structure_Should_Build(string architecture, string[] commands)
        {
            // Arrange
            string projectDir = GetProjectPath(DefaultProjectName);

            // Act
            int createResult = RunCommand("apiand",
                $"new {architecture} --output {projectDir} --name {DefaultProjectName} --skip-interactive");
            Assert.Equal(0, createResult);

            // Execute all commands sequentially
            foreach (string cmd in commands)
            {
                int cmdResult = RunCommand("apiand", $"generate {cmd} -o {projectDir}");
                Assert.Equal(0, cmdResult);
            }

            // Assert
            Assert.True(VerifyBuild(projectDir));
        }

        [Theory]
        [InlineData("microservices", "Catalog", "Orders", "Shipping")]
        public void Microservices_With_Multiple_Projects_Should_Build(string architecture, params string[] projects)
        {
            // Arrange
            string projectDir = GetProjectPath(DefaultProjectName);

            // Act
            int createResult = RunCommand("apiand",
                $"new {architecture} --output {projectDir} --name {DefaultProjectName} --skip-interactive");
            Assert.Equal(0, createResult);

            // Create multiple projects
            foreach (string project in projects)
            {
                int projectResult = RunCommand("apiand", $"generate project {project} -o {projectDir}");
                Assert.Equal(0, projectResult);

                // Add a service and endpoint to each project
                int serviceResult =
                    RunCommand("apiand", $"generate service {project}.Core -o {projectDir}");
                Assert.Equal(0, serviceResult);

                int endpointResult = RunCommand("apiand",
                    $"generate endpoint Get{project} --http-method Get -o {projectDir}");
                Assert.Equal(0, endpointResult);
            }

            // Assert
            Assert.True(VerifyBuild(projectDir));
        }

        [Theory]
        [InlineData("ddd", "--presentation FastEndpoints", "service", "endpoint", "entity")]
        [InlineData("single-layer", "", "service", "endpoint", "entity")]
        public void Different_Component_Types_With_Options_Should_Build(string architecture, string options,
            string componentType1, string componentType2, string componentType3)
        {
            // Arrange
            string projectDir = GetProjectPath(DefaultProjectName);

            // Act
            int createResult = RunCommand("apiand",
                $"new {architecture} --output {projectDir} --name {DefaultProjectName} --skip-interactive {options}"
                    .Trim());
            Assert.Equal(0, createResult);

            // Generate different component types
            string[] components =
            {
                $"{componentType1} Test.First",
                $"{componentType2} GetTest --http-method Get",
                $"{componentType3} Test.Item --attributes \"id:int;name:string\""
            };

            foreach (string component in components)
            {
                int generateResult = RunCommand("apiand", $"generate {component} -o {projectDir}");
                Assert.Equal(0, generateResult);
            }

            // Assert
            Assert.True(VerifyBuild(projectDir));
        }
    }
}