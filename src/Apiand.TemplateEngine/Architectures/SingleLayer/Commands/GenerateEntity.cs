using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using Apiand.TemplateEngine.Utils;
using System.Text;
using Apiand.Extensions.Models;

namespace Apiand.TemplateEngine.Architectures.SingleLayer.Commands;

public class GenerateEntity : IGenerateEntity
{
    public string ArchName { get; set; } = SingleLayerUtils.Name;

    public Result Handle(string workingDirectory, string projectDirectory, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var entityName = argument;

        // Parse the entity name for subdirectories (e.g., "Identity.User" -> ["Identity", "User"])
        string[] nameParts = entityName.Split('.');
        string className = nameParts[^1]; // Last part is the actual entity name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));

        // Find the main project
        string mainProject = null;
        var projectFiles = Directory.GetFiles(projectDirectory, "*.csproj", SearchOption.AllDirectories);

        // Get the first project file or the one matching the configuration project name
        foreach (var projectFile in projectFiles)
        {
            string projectFileName = Path.GetFileNameWithoutExtension(projectFile);
            if (projectFileName.Equals(configuration.ProjectName, StringComparison.OrdinalIgnoreCase))
            {
                mainProject = Path.GetDirectoryName(projectFile);
                break;
            }
            
            // If no specific match, just pick the first one
            if (mainProject == null)
            {
                mainProject = Path.GetDirectoryName(projectFile);
            }
        }

        if (mainProject == null)
        {
            return Result.Fail(TemplatingErrors.ProjectNotFound);
        }

        // Create entity directory
        string entityDir = Path.Combine(mainProject, "Entities", subDirPath);
        Directory.CreateDirectory(entityDir);

        // Parse attributes if provided
        List<EntityAttribute> attributes = new();
        if (extraData.TryGetValue("attributes", out string attributesString))
        {
            attributes = EntityAttribute.ParseAttributes(attributesString);
        }

        // Generate properties for entity
        var propertiesBuilder = EntityAttribute.BuildPropertiesString(attributes, className);

        // Generate entity class using CodeBlocks
        string entityContent = CodeBlocks.GenerateEntityClass(
            className, 
            configuration.ProjectName, 
            subDirPath, 
            propertiesBuilder.ToString());

        // Write the entity file
        string entityPath = Path.Combine(entityDir, $"{className}.cs");
        File.WriteAllText(entityPath, entityContent);

        messenger.WriteStatusMessage($"Created entity at {Path.GetRelativePath(projectDirectory, entityPath)}");

        // Generate enums if needed
        foreach (var enumAttribute in attributes.Where(a => a.Type.StartsWith("enum[")))
        {
            string enumName = $"{className}{EntityAttribute.CapitalizeFirst(enumAttribute.Name)}";
            
            // Generate enum values
            var enumValuesBuilder = EntityAttribute.BuildEnumValuesString(enumAttribute.EnumValues);
            
            string enumContent = CodeBlocks.GenerateEnumClass(
                enumName, 
                configuration.ProjectName, 
                subDirPath, 
                enumValuesBuilder.ToString());
                
            string enumPath = Path.Combine(entityDir, $"{enumName}.cs");
            File.WriteAllText(enumPath, enumContent);
            messenger.WriteStatusMessage($"Created enum at {Path.GetRelativePath(projectDirectory, enumPath)}");
        }
        
        return Result.Succeed();
    }
}
