namespace Apiand.TemplateEngine.Architectures.DDD;

public enum Option
{
    Default,
    MediatR,
    FastEndpoints,
    EFCore,
    MongoDB,
}

public static class DddOptions
{
    private static readonly Dictionary<Layer, Option[]> Options = new()
    {
        { Layer.Application, [Option.MediatR] },
        { Layer.Domain, [Option.Default] },
        { Layer.Presentation, [Option.Default, Option.FastEndpoints] },
        { Layer.Infrastructure, [Option.EFCore, Option.MongoDB] },
        { Layer.AppHost, [Option.Default] },
        { Layer.ServiceDefaults, [Option.Default] }
    };

    public static Option[] GetByLayer(Layer layer)
    {
        return Options[layer];
    }
}