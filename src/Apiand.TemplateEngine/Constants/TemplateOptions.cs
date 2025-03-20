using Apiand.TemplateEngine.Options;
using Apiand.TemplateEngine.Utils;

namespace Apiand.TemplateEngine.Constants;

public static class TemplateOptions
{
    public static readonly Architecture[] ArchitectureTypes = EnumUtils.GetAll<Architecture>();
    public static readonly Module[] Modules = EnumUtils.GetAll<Module>();
    public static readonly Presentation[] PresentationTypes = EnumUtils.GetAll<Presentation>();
    public static readonly Application[] ApplicationLayerTypes = EnumUtils.GetAll<Application>();
    public static readonly Domain[] DomainTypes = EnumUtils.GetAll<Domain>();
    public static readonly Infrastructure[] InfrastructureTypes = EnumUtils.GetAll<Infrastructure>();

    public static readonly Dictionary<Architecture, List<Presentation>> ValidEndpointTypes = new()
    {
        [Architecture.MultiLayer] = [Presentation.Rest, Presentation.Default],
    };

    public static readonly Dictionary<Architecture, List<Infrastructure>> ValidDbTypes = new()
    {
        [Architecture.MultiLayer] = [Infrastructure.MongoDB],
    };
}