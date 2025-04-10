using DotNetCore.CAP;
using XXXnameXXX.Inventory.Models;
using XXXnameXXX.Shared.Data;
using XXXnameXXX.Shared.Events;

namespace XXXnameXXX.Inventory.EventHandlers;

public class OrderCreatedEventHandler : ICapSubscribe
{
    private readonly IRepository<Product> _repository;

    public OrderCreatedEventHandler(IRepository<Product> repository)
    {
        _repository = repository;
    }

    [CapSubscribe("order.created")]
    public async Task HandleOrderCreated(OrderCreatedEvent @event)
    {
        Console.WriteLine("Got here");

        // Process each product in the order
        foreach (var item in @event.Items)
        {
            var product = await _repository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                // Adjust inventory
                product.AvailableStock -= item.Quantity;
                await _repository.UpdateAsync(product.Id, product);
            }
        }
    }
}