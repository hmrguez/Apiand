// XXXnameXXX.Orders/Models/Order.cs

using Apiand.Extensions.DDD;

namespace XXXnameXXX.Orders.Models;

public class Order : Entity
{
    public string OrderNumber { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public OrderStatus Status { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public enum OrderStatus
{
    Created,
    Processing,
    Completed,
    Cancelled
}