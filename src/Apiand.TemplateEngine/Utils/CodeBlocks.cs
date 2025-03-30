namespace Apiand.TemplateEngine.Utils;

public static class CodeBlocks
{
    // Endpoint generation templates
    public static string GenerateEndpointRequest(string projectName, string endpointClassName, string requestTypeName, string subDirPath, bool isQuery)
    {
        return $$"""
        using MediatR;
        using Apiand.Extensions.Models;
        using {{projectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

        namespace {{projectName}}.Application.{{(isQuery ? "Queries" : "Commands")}}{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}}.{{endpointClassName}};

        public class {{requestTypeName}}: IRequest<Result<{{endpointClassName}}Response>>
        {
            // TODO: Add {{(isQuery ? "query" : "command")}} properties
        }
        """;
    }

    public static string GenerateEndpointHandler(string projectName, string endpointClassName, string requestTypeName, string subDirPath, bool isQuery)
    {
        return $$"""
        using Apiand.Extensions.Models;
        using MediatR;
        using Microsoft.Extensions.Logging;
        using {{projectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

        namespace {{projectName}}.Application.{{(isQuery ? "Queries" : "Commands")}}{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}}.{{endpointClassName}};

        public class {{requestTypeName}}Handler(ILogger<{{requestTypeName}}Handler> logger) : IRequestHandler<{{requestTypeName}}, Result<{{endpointClassName}}Response>>
        {
            public async Task<Result<{{endpointClassName}}Response>> Handle({{requestTypeName}} request, CancellationToken cancellationToken)
            {
                logger.LogInformation("Processing {{requestTypeName}}");
                
                // TODO: Implement handler logic
                
                return new {{endpointClassName}}Response();
            }
        }
        """;
    }

    public static string GenerateEndpointResponse(string projectName, string endpointClassName, string subDirPath)
    {
        return $$"""
        namespace {{projectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

        public class {{endpointClassName}}Response
        {
            // TODO: Add response properties
        }
        """;
    }

    public static string GenerateEndpointClass(string projectName, string endpointClassName, string requestTypeName, string subDirPath, string httpMethodStr, bool isQuery)
    {
        return $$"""
        using {{projectName}}.Application.{{(isQuery ? "Queries" : "Commands")}}{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}}.{{endpointClassName}};
        using {{projectName}}.Application.Dtos{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
        using MediatR;
        using {{projectName}}.Presentation.Models;

        namespace {{projectName}}.Presentation.Endpoints{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

        public class {{endpointClassName}}Endpoint(IMediator mediator) : CustomEndpoint<{{requestTypeName}}, {{endpointClassName}}Response>(mediator)
        {
            protected override string Route => "{{endpointClassName.ToLowerInvariant()}}";
            protected override Models.HttpMethod Method => Models.HttpMethod.{{httpMethodStr}};
            protected override bool Secure => {{(isQuery ? "false" : "true")}};
        }
        """;
    }

    // Entity generation templates
    public static string GenerateEntityClass(string className, string projectName, string subDirPath, string properties)
    {
        return $$"""
        using Apiand.Extensions.DDD;

        namespace {{projectName}}.Domain.Entities{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

        public class {{className}} : Entity
        {
            {{properties}}
        }
        """;
    }

    public static string GenerateEnumClass(string enumName, string projectName, string subDirPath, string enumValues)
    {
        return $$"""
        namespace {{projectName}}.Domain.Entities{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

        public enum {{enumName}}
        {
            {{enumValues}}
        }
        """;
    }

    // Service generation templates
    public static string GenerateServiceInterface(string projectName, string serviceClassName, string subDirPath)
    {
        return $$"""
        namespace {{projectName}}.Application.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
                
        public interface I{{serviceClassName}}Service
        {
            // TODO: Add service methods
        }
        """;
    }

    public static string GenerateServiceImplementation(string projectName, string serviceClassName, string subDirPath)
    {
        return $$"""
        using {{projectName}}.Application.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};
        using Apiand.Extensions.Service;

        namespace {{projectName}}.Infrastructure.Services{{(subDirPath.Length > 0 ? "." + subDirPath.Replace("/", ".") : "")}};

        [Service]
        public class {{serviceClassName}}Service : I{{serviceClassName}}Service
        {
            // TODO: Implement service methods
        }
        """;
    }
}
