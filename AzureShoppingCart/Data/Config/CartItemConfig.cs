using AzureShoppingCart.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureShoppingCart.Data.Config;

public sealed class CartItemConfig : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(x => x.Id)
            .HasName("PK_CartItems");

        builder.Property(x => x.CartId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasIndex(x => new { x.CartId, x.ProductId })
            .HasDatabaseName("UQ_CartItems_CartId_ProductId")
            .IsUnique();
    }
}
