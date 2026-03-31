namespace AzureShoppingCart.Entities;

public sealed class OrderItem
{
    public Guid Id { get; set; }
    public required Guid OrderId { get; set; }
    public required int ProductId { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public required string ProductName { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
