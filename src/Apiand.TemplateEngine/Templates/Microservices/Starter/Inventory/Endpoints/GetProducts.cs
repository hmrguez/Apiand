using FastEndpoints;
using XXXnameXXX.Inventory.Models;
using XXXnameXXX.Shared.Data;

namespace XXXnameXXX.Inventory.Endpoints;

public class GetProducts : EndpointWithoutRequest<IEnumerable<Product>>
{
    private readonly IRepository<Product> _repository;

    public GetProducts(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/products");
        AllowAnonymous();
        Description(b => b.WithName("GetProducts"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var products = await _repository.GetAllAsync();
        await SendAsync(products, cancellation: ct);
    }
}