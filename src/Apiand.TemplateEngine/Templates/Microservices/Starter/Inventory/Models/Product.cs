using Apiand.Extensions.DDD;

namespace XXXnameXXX.Inventory.Models;

public class Product : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int AvailableStock { get; set; }
    public decimal Price { get; set; }
}