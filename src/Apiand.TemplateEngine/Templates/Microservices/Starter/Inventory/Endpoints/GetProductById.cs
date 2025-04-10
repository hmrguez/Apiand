using FastEndpoints;
using XXXnameXXX.Inventory.Models;
using XXXnameXXX.Shared.Data;

namespace XXXnameXXX.Inventory.Endpoints;

public class GetProductByIdRequest
{
    public string Id { get; set; }
}

public class GetProductById : Endpoint<GetProductByIdRequest, Product>
{
    private readonly IRepository<Product> _repository;

    public GetProductById(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/products/{Id}");
        AllowAnonymous();
        Description(b => b.WithName("GetProduct"));
    }

    public override async Task HandleAsync(GetProductByIdRequest req, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(req.Id);

        if (product == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(product, cancellation: ct);
    }
}