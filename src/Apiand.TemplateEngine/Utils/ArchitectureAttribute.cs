namespace Apiand.TemplateEngine.Utils;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ArchitectureAttribute(string architectureName) : Attribute
{
    public string ArchitectureName { get; } = architectureName;
}