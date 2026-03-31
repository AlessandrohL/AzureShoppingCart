using AzureShoppingCart.Data.Interfaces;

namespace AzureShoppingCart.Entities;

public sealed class Customer : IAuditable
{
    public Guid Id { get; set; }
    public required string IdentityId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Order> Orders { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
