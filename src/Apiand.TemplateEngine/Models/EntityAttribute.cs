using System.Text;
using System.Text.RegularExpressions;

namespace Apiand.TemplateEngine.Models;

public class EntityAttribute
{
    public string Name { get; set; }
    public string Type { get; set; }
    public List<string> EnumValues { get; set; } = new List<string>();

    public static List<EntityAttribute> ParseAttributes(string attributesString)
    {
        var attributes = new List<EntityAttribute>();
        var attributePairs = attributesString?.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in attributePairs ?? [])
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

    public static string GetPropertyType(EntityAttribute attr, string entityName)
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

    public static string CapitalizeFirst(string input) =>
        input.Length > 0 ? char.ToUpper(input[0]) + input.Substring(1) : input;

    public static StringBuilder BuildPropertiesString(List<EntityAttribute> attributes, string className)
    {
        var propertiesBuilder = new StringBuilder();
        foreach (var attr in attributes)
        {
            string propertyType = GetPropertyType(attr, className);
            propertiesBuilder.AppendLine($"    public {propertyType} {CapitalizeFirst(attr.Name)} {{ get; set; }}");
        }
        return propertiesBuilder;
    }

    public static StringBuilder BuildEnumValuesString(List<string> enumValues)
    {
        var enumValuesBuilder = new StringBuilder();
        if (enumValues.Count > 0)
        {
            for (int i = 0; i < enumValues.Count; i++)
            {
                string value = enumValues[i];
                enumValuesBuilder.AppendLine($"    {CapitalizeFirst(value)}{(i < enumValues.Count - 1 ? "," : "")}");
            }
        }
        return enumValuesBuilder;
    }
}
