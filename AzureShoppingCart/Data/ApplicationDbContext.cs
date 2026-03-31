using AzureShoppingCart.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AzureShoppingCart.Data;

public sealed class ApplicationDbContext(DbContextOptions options) 
    : IdentityDbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityUser>().ToTable("IdentityUsers");
        builder.Entity<IdentityRole>().ToTable("IdentityRoles");
        builder.Entity<IdentityUserRole<string>>().ToTable("IdentityUserRoles");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("IdentityRoleClaims");
        builder.Entity<IdentityUserClaim<string>>().ToTable("IdentityUserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("IdentityUserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("IdentityUserTokens");

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
