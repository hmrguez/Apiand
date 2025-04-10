using DotNetCore.CAP;
using FastEndpoints;
using XXXnameXXX.Orders.Models;
using XXXnameXXX.Shared.Data;
using XXXnameXXX.Shared.Events;
using Order = XXXnameXXX.Orders.Models.Order;

namespace XXXnameXXX.Orders.Endpoints;

public class CreateOrderRequest
{
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CreateOrder(IRepository<Order> repository, ICapPublisher capPublisher)
    : Endpoint<CreateOrderRequest, Order>
{
    public override void Configure()
    {
        Post("/orders");
        AllowAnonymous();
        Description(b => b.WithName("CreateOrder"));
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        // Create a new order
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}",
            Status = OrderStatus.Created,
            Items = req.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

        // Save to database
        await repository.InsertAsync(order);

        // Publish event
        await capPublisher.PublishAsync("order.created", new OrderCreatedEvent
        {
            OrderId = order.Id,
            Items = order.Items.Select(i => new OrderItemEvent
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        }, cancellationToken: ct);

        await SendCreatedAtAsync($"/orders/{order.Id}", order, order, cancellation: ct);
    }
}