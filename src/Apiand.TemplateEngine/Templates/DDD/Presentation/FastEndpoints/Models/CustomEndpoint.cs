using System.Net;
using Apiand.Extensions.Models;
using FastEndpoints;
using MediatR;

namespace XXXnameXXX.Presentation.Models;

public abstract class CustomEndpoint<TRequest, TResult>(IMediator mediator) : Endpoint<TRequest, Result<TResult>>
    where TRequest : IRequest<Result<TResult>>
{
    public override void Configure()
    {
        if (Method == Models.HttpMethod.Get)
            Get(Route);
        else if (Method == Models.HttpMethod.Post)
            Post(Route);
        else if (Method == Models.HttpMethod.Put)
            Put(Route);
        else if (Method == Models.HttpMethod.Delete)
            Delete(Route);
        else if (Method == Models.HttpMethod.Patch)
            Patch(Route);
        else
            throw new ArgumentOutOfRangeException($"Method not supported");

        if (!Secure)
            AllowAnonymous();
        
        ExtraConfigure();
    }

    public override async Task HandleAsync(TRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        var statusCode = (int)(result.Error?.StatusCode ?? HttpStatusCode.OK);
        await SendAsync(result, statusCode, cancellation: ct);
    }

    protected abstract string Route { get; }
    protected abstract HttpMethod Method { get; }
    protected abstract bool Secure { get; }

    protected virtual void ExtraConfigure()
    {
        
    }
}