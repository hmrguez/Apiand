namespace Apiand.TemplateEngine.Constants;

public static class Options
{
    public static readonly string[] ArchitectureTypes = ["multi-layer",];
    public static readonly string[] Modules = ["application", "domain", "infrastructure", "endpoints"];
    public static readonly string[] EndpointTypes = ["graphql"];
    public static readonly string[] InfrastructureTypes = ["mongodb"];

    public static readonly Dictionary<string, List<string>> ValidEndpointTypes = new()
    {
        ["multi-layer"] = ["graphql"],
    };

    public static readonly Dictionary<string, List<string>> ValidDbTypes = new()
    {
        ["multi-layer"] = ["mongodb"],
    };
}