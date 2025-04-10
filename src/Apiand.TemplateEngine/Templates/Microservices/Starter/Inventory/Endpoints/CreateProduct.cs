using FastEndpoints;
using XXXnameXXX.Inventory.Models;
using XXXnameXXX.Shared.Data;

namespace XXXnameXXX.Inventory.Endpoints;

public class CreateProduct : Endpoint<Product, Product>
{
    private readonly IRepository<Product> _repository;

    public CreateProduct(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/products");
        AllowAnonymous();
        Description(b => b.WithName("CreateProduct"));
    }

    public override async Task HandleAsync(Product product, CancellationToken ct)
    {
        await _repository.InsertAsync(product);
        await SendCreatedAtAsync($"/products/{product.Id}", product, product, cancellation: ct);
    }
}