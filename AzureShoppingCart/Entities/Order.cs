using AzureShoppingCart.Data.Interfaces;

namespace AzureShoppingCart.Entities;

public sealed class Order : IAuditable
{
    public Guid Id { get; set; }
    public required Guid CustomerId { get; set; }
    public required string OrderNumber { get; set; }
    public required OrderStatus Status { get; set; }
    public required decimal TotalAmount { get; set; }
    public required DateTimeOffset OrderDate { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
