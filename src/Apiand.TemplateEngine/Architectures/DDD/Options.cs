namespace Apiand.TemplateEngine.Architectures.DDD;

public enum Option
{
    Default,
    FastEndpoints,
    MongoDB,
}

public static class DddOptions
{
    private static readonly Dictionary<Layer, Option[]> Options = new()
    {
        { Layer.Application, [Option.Default] },
        { Layer.Domain, [Option.Default] },
        { Layer.Presentation, [Option.Default, Option.FastEndpoints] },
        { Layer.Infrastructure, [Option.Default, Option.MongoDB] }
    };

    public static Option[] GetByLayer(Layer layer)
    {
        return Options[layer];
    }
}