using Apiand.TemplateEngine.Options;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Constants;

public static class TemplateOptions
{
    public static readonly Architecture[] ArchitectureTypes = EnumUtils.GetAll<Architecture>();
    public static readonly Module[] Modules = EnumUtils.GetAll<Module>();
    public static readonly Endpoint[] EndpointTypes = EnumUtils.GetAll<Endpoint>();
    public static readonly Infrastructure[] InfrastructureTypes = EnumUtils.GetAll<Infrastructure>();

    public static readonly Dictionary<Architecture, List<Endpoint>> ValidEndpointTypes = new()
    {
        [Architecture.MultiLayer] = [Endpoint.GraphQL],
    };

    public static readonly Dictionary<Architecture, List<Infrastructure>> ValidDbTypes = new()
    {
        [Architecture.MultiLayer] = [Infrastructure.MongoDB],
    };
}