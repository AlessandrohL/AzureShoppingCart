using AzureShoppingCart.Data;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Identity.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AzureShoppingCart.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application Database migrations applied successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "An error occurred while applying database migrations.");
            throw;
        }
    }

    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        SeedAdminOptions defaultAdmin = scope.ServiceProvider.GetRequiredService<IOptions<SeedAdminOptions>>().Value;

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            if (!await roleManager.RoleExistsAsync(AuthRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(AuthRoles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(AuthRoles.Customer))
            {
                await roleManager.CreateAsync(new IdentityRole(AuthRoles.Customer));
            }

            app.Logger.LogInformation("Succesfully created roles.");

            await SeedAdminUserAsync(context, userManager, defaultAdmin);

            app.Logger.LogInformation("Succesfully seed admin");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while seeding initial data.");
            throw;
        }
    }

    private static async Task SeedAdminUserAsync(
        ApplicationDbContext dbContext,
        UserManager<IdentityUser> userManager,
        SeedAdminOptions seedAdmin)
    {
        var existingUser = await userManager.FindByEmailAsync(seedAdmin.Email);

        if (existingUser is not null)
        {
            return;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var user = new IdentityUser
        {
            UserName = seedAdmin.Email,
            Email = seedAdmin.Email,
            EmailConfirmed = true,
        };

        IdentityResult createUserResult = await userManager.CreateAsync(user, seedAdmin.Password);

        if (!createUserResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create admin user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
        }

        IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, AuthRoles.Admin);

        if (!addRoleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create admin user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
        }

        await transaction.CommitAsync();
    }
}
