using AzureShoppingCart.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace AzureShoppingCart.Identity;

public sealed class ClaimsIdentityProvider(
    UserManager<IdentityUser> userManager,
    ApplicationDbContext dbContext)
{
    private const string RoleClaimName = "role"; 

    public async Task<ClaimsIdentity> CreateClaims(IdentityUser user)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);

        var claims = new ClaimsIdentity(
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(
                JwtRegisteredClaimNames.EmailVerified, 
                user.EmailConfirmed.ToString().ToLowerInvariant(), 
                ClaimValueTypes.Boolean),
        ]);

        claims.AddClaims(roles.Select(role => new Claim(RoleClaimName, role)));

        if (roles.Contains(AuthRoles.Customer))
        {
            Guid customerId = await dbContext.Customers
                .Where(c => c.IdentityId == user.Id)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            claims.AddClaim(new Claim(CustomClaims.CustomerIdentifier, customerId.ToString()));
        }

        return claims;
    }
}
