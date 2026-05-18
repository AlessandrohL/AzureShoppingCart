using System.Text.Json.Serialization;

namespace AzureShoppingCart.Features.ShoppingCart;

public sealed class ShoppingCartCache
{
    public string? Id { get; init; }
    public List<ShoppingCartItemCache> Items { get; init; } = [];

    public static ShoppingCartCache Empty()
    {
        return new ShoppingCartCache();
    }

    [JsonIgnore]
    public bool IsEmpty => Items.Count == 0;
}

public sealed class ShoppingCartItemCache
{
    public int Id { get; init; }
    public required string ProductName { get; init; }
    public required int ProductId { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
}