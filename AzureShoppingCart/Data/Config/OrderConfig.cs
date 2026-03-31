using AzureShoppingCart.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureShoppingCart.Data.Config;

public sealed class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id)
            .HasName("PK_Orders");

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(o => o.OrderNumber)
            .HasDatabaseName("UQ_Orders_OrderNumber")
            .IsUnique();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasDefaultValue(OrderStatus.Pending, "DF_Orders_Status");

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.HasIndex(o => o.OrderDate)
            .HasDatabaseName("IX_Orders_OrderDate");

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .HasConstraintName("FK_Orders_Customers");

        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("IX_Orders_CustomerId");

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(100);
    }
}
