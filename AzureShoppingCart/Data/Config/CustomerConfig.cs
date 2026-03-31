using AzureShoppingCart.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureShoppingCart.Data.Config;

public sealed class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id)
            .HasName("PK_Customers");

        builder.Property(c => c.IdentityId)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(c => c.IdentityId)
            .HasDatabaseName("UQ_Customers_IdentityId")
            .IsUnique();

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(c => c.Email)
            .HasDatabaseName("UQ_Customers_Email")
            .IsUnique();

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Ignore(c => c.FullName);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(100);
    }
}
