using FastEndpoints;
using XXXnameXXX.Orders.Models;
using XXXnameXXX.Shared.Data;
using Order = XXXnameXXX.Orders.Models.Order;

namespace XXXnameXXX.Orders.Endpoints;

public class GetOrderRequest
{
    public string Id { get; set; }
}

public class GetOrder(IRepository<Order> repository) : Endpoint<GetOrderRequest, Order>
{
    public override void Configure()
    {
        Get("/orders/{Id}");
        AllowAnonymous();
        Description(b => b.WithName("GetOrder"));
    }

    public override async Task HandleAsync(GetOrderRequest req, CancellationToken ct)
    {
        var order = await repository.GetByIdAsync(req.Id);
        
        if (order == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(order, cancellation: ct);
    }
}