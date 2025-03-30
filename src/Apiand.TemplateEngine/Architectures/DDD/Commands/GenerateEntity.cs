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
        var attributePairs = attributesString.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in attributePairs)
        {
            var parts = pair.Trim().Split(':', 2);
            if (parts.Length == 2)
            {
                string name = parts[0].Trim();
                string type = parts[1].Trim();

                // Extract enum values if it's an enum type
                List<string> enumValues = new List<string>();
                if (type.StartsWith("enum[") && type.EndsWith("]"))
                {
                    // Parse enum values from format: enum[value1,value2,value3]
                    var enumValueMatch = Regex.Match(type, @"enum\[(.*)\]");
                    if (enumValueMatch.Success)
                    {
                        string valuesStr = enumValueMatch.Groups[1].Value;
                        enumValues = valuesStr.Split(',').Select(v => v.Trim()).ToList();
                    }
                }

                attributes.Add(new EntityAttribute 
                { 
                    Name = name, 
                    Type = type,
                    EnumValues = enumValues
                });
            }
        }

        return attributes;
    }

    private string GenerateEntityClass(string className, string projectName, string subDirPath, List<EntityAttribute> attributes)
    {
        var sb = new StringBuilder();
        string @namespace = $"{projectName}.Domain.Entities{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}";
    
        sb.AppendLine("using Apiand.Extensions.DDD;");
        sb.AppendLine();
        sb.AppendLine($"namespace {@namespace};");
        sb.AppendLine();
        sb.AppendLine($"public class {className} : Entity");
        sb.AppendLine("{");
    
        // Add all specified attributes
        foreach (var attr in attributes)
        {
            string propertyType = GetPropertyType(attr, className);
            sb.AppendLine($"    public {propertyType} {CapitalizeFirst(attr.Name)} {{ get; set; }}");
        }
    
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

        // Add enum values with proper formatting
        if (attribute.EnumValues.Count > 0)
        {
            for (int i = 0; i < attribute.EnumValues.Count; i++)
            {
                string value = attribute.EnumValues[i];
                sb.AppendLine($"    {CapitalizeFirst(value)}{(i < attribute.EnumValues.Count - 1 ? "," : "")}");
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
            "datetime" => "DateTime",
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
        public List<string> EnumValues { get; set; } = new List<string>();
    }
}