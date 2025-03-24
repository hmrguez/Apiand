using System.Reflection;
using Mapster;

namespace XXXnameXXX.Application.DI;

public static class MappingInstaller
{
    public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(Assembly.GetExecutingAssembly());
    }
}