using Apiand.TemplateEngine.Models;
using Apiand.TemplateEngine.Models.Commands;
using System.Text;
using System.Text.RegularExpressions;

namespace Apiand.TemplateEngine.Architectures.DDD.Commands;

public class GenerateEntity : IGenerateEntity
{
    public string ArchName { get; set; } = DddUtils.Name;

    public void Handle(string workingDirectory, string projectDirectory, string argument, 
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger)
    {
        var entityName = argument;

        // Parse the entity name for subdirectories (e.g., "Identity.User" -> ["Identity", "User"])
        string[] nameParts = entityName.Split('.');
        string className = nameParts[^1]; // Last part is the actual entity name
        string subDirPath = string.Join("/", nameParts.Take(nameParts.Length - 1));

        // Find the Domain project
        string domainProject = null;
        var projectFiles = Directory.GetFiles(projectDirectory, "*.csproj", SearchOption.AllDirectories);

        foreach (var projectFile in projectFiles)
        {
            string projectFileName = Path.GetFileNameWithoutExtension(projectFile);
            if (projectFileName.EndsWith("Domain"))
            {
                domainProject = Path.GetDirectoryName(projectFile);
                break;
            }
        }

        if (domainProject == null)
        {
            messenger.WriteErrorMessage("Could not find Domain project in DDD architecture.");
            return;
        }

        // Create entity directory
        string entityDir = Path.Combine(domainProject, "Entities", subDirPath);
        Directory.CreateDirectory(entityDir);

        // Parse attributes if provided
        List<EntityAttribute> attributes = new();
        if (extraData.TryGetValue("attributes", out string attributesString))
        {
            attributes = ParseAttributes(attributesString);
        }

        // Generate entity class content
        string entityContent = GenerateEntityClass(className, configuration.ProjectName, subDirPath, attributes);
        
        // Write the entity file
        string entityPath = Path.Combine(entityDir, $"{className}.cs");
        File.WriteAllText(entityPath, entityContent);

        messenger.WriteStatusMessage($"Created entity at {Path.GetRelativePath(projectDirectory, entityPath)}");
        
        // Generate enums if needed
        foreach (var enumAttribute in attributes.Where(a => a.Type.StartsWith("enum[")))
        {
            string enumName = $"{className}{char.ToUpper(enumAttribute.Name[0])}{enumAttribute.Name.Substring(1)}";
            string enumContent = GenerateEnumClass(enumName, configuration.ProjectName, subDirPath, enumAttribute);
            string enumPath = Path.Combine(entityDir, $"{enumName}.cs");
            File.WriteAllText(enumPath, enumContent);
            messenger.WriteStatusMessage($"Created enum at {Path.GetRelativePath(projectDirectory, enumPath)}");
        }
    }

    private List<EntityAttribute> ParseAttributes(string attributesString)
    {
        var attributes = new List<EntityAttribute>();
        var attributePairs = attributesString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var pair in attributePairs)
        {
            var parts = pair.Trim().Split(':', 2);
            if (parts.Length == 2)
            {
                string name = parts[0].Trim();
                string type = parts[1].Trim();
                
                attributes.Add(new EntityAttribute { Name = name, Type = type });
            }
        }
        
        return attributes;
    }

    private string GenerateEntityClass(string className, string projectName, string subDirPath, List<EntityAttribute> attributes)
    {
        var sb = new StringBuilder();
        string @namespace = $"{projectName}.Domain.Entities{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";

        sb.AppendLine($"namespace {@namespace};");
        sb.AppendLine();
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");
        
        // Add Id property
        sb.AppendLine("    public Guid Id { get; set; }");
        
        // Add all specified attributes
        foreach (var attr in attributes)
        {
            string propertyType = GetPropertyType(attr, className);
            sb.AppendLine($"    public {propertyType} {CapitalizeFirst(attr.Name)} {{ get; set; }}");
        }
        
        // Add timestamp properties
        sb.AppendLine("    public DateTime CreatedAt { get; set; }");
        sb.AppendLine("    public DateTime? UpdatedAt { get; set; }");
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }

    private string GenerateEnumClass(string enumName, string projectName, string subDirPath, EntityAttribute attribute)
    {
        var sb = new StringBuilder();
        string @namespace = $"{projectName}.Domain.Entities{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";

        sb.AppendLine($"namespace {@namespace};");
        sb.AppendLine();
        sb.AppendLine($"public enum {enumName}");
        sb.AppendLine("{");

        // Extract enum values from enum[value1,value2,...] format
        var match = Regex.Match(attribute.Type, @"enum\[(.*)\]");
        if (match.Success)
        {
            string[] values = match.Groups[1].Value.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i].Trim();
                sb.AppendLine($"    {CapitalizeFirst(value)}{(i < values.Length - 1 ? "," : "")}");
            }
        }
        
        sb.AppendLine("}");
        
        return sb.ToString();
    }

    private string GetPropertyType(EntityAttribute attr, string entityName)
    {
        if (attr.Type.StartsWith("enum["))
        {
            // For enum types, reference the entity-specific enum
            return $"{entityName}{CapitalizeFirst(attr.Name)}";
        }
        
        return attr.Type.ToLower() switch
        {
            "string" => "string",
            "int" => "int",
            "long" => "long",
            "double" => "double",
            "decimal" => "decimal",
            "bool" => "bool",
            "boolean" => "bool",
            "datetime" => "DateTime",
            "date" => "DateTime",
            "guid" => "Guid",
            _ => "string" // default to string for unknown types
        };
    }

    private string CapitalizeFirst(string input) => 
        input.Length > 0 ? char.ToUpper(input[0]) + input.Substring(1) : input;

    private class EntityAttribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}