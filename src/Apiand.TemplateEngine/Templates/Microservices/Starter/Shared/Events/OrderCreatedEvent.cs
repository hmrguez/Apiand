namespace XXXnameXXX.Shared.Events;

public class OrderCreatedEvent
{
    public string OrderId { get; set; }
    public List<OrderItemEvent> Items { get; set; } = new();
}

public class OrderItemEvent
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}