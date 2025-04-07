using System.Diagnostics;

namespace XXXnameXXX.AppHost;

internal static class OpenApiExtensions
{
    public static IResourceBuilder<T> WithSwaggerUi<T>(this IResourceBuilder<T> builder) where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("swagger", "Swagger", "swagger");
    }
    
    public static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder) where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("scalar", "Scalar", "scalar");
    } 
    
    private static IResourceBuilder<T> WithOpenApiDocs<T>(this IResourceBuilder<T> builder, string name,
        string displayName, string openApiUiPath) where T : IResourceWithEndpoints
    {
        return builder.WithCommand(name, displayName, async _ =>
        {
            try
            {
                var endpoint = builder.GetEndpoint("http");
                var url = $"{endpoint.Url}/{openApiUiPath}";

                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return new ExecuteCommandResult { Success = true };
            }
            catch (Exception e)
            {
                return new ExecuteCommandResult { Success = false, ErrorMessage = e.ToString() };
            }
        });
    }
}