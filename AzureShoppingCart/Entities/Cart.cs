namespace AzureShoppingCart.Entities;

public sealed class Cart
{
    public Guid Id { get; set; }
    public required Guid CustomerId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = [];

    public void AddItem(int productId, int quantity)
    {
        Items.Add(new CartItem
        {
            CartId = Id,
            ProductId = productId,
            Quantity = quantity
        });
    }

    public static Cart Create(Guid customerId, TimeProvider timeProvider)
    {
        return new Cart
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CreatedAt = timeProvider.GetUtcNow()
        };
    }
}
