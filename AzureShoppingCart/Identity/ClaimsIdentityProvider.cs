using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace AzureShoppingCart.Identity;

public sealed class ClaimsIdentityProvider(UserManager<IdentityUser> userManager)
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

        return claims;
    }
}
