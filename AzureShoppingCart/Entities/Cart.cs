namespace AzureShoppingCart.Entities;

public sealed class Cart
{
    public Guid Id { get; set; }
    public required string CustomerId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = [];
}
