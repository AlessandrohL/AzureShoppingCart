namespace AzureShoppingCart.Identity;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
}
