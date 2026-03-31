namespace AzureShoppingCart.Entities;

public sealed class CartItem
{
    public int Id { get; set; }
    public required Guid CartId { get; set; }
    public required int ProductId { get; set; }
    public required uint Quantity { get; set; }

    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
}
