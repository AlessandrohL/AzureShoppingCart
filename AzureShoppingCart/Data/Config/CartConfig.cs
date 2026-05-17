using AzureShoppingCart.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureShoppingCart.Data.Config;

public sealed class CartConfig : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id)
            .HasName("PK_Carts");

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.CartId)
            .HasConstraintName("FK_Cart_Items")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("UQ_Carts_CustomerId")
            .IsUnique();
    }
}
