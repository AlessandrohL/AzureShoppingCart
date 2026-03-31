using AzureShoppingCart.Data.Interfaces;

namespace AzureShoppingCart.Entities;

public sealed class Product : IAuditable
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Slug { get; set; }
    public required int BrandId { get; set; }
    public required decimal Price { get; set; }
    public required int Stock { get; set; }
    public required string ImageUrl { get; set; }

    public Brand? Brand { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
