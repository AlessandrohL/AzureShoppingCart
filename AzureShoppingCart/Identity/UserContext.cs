using System.Security.Claims;

namespace AzureShoppingCart.Identity;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor)
    : IUserContext
{
    public string? UserId =>
        httpContextAccessor
            .HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated =>
        httpContextAccessor
            .HttpContext?
            .User?
            .Identity?
            .IsAuthenticated ?? false;
}
