namespace AzureShoppingCart.Options;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
    public int ExpirationInMinutes { get; init; } = default;
}
