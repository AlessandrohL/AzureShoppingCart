namespace AzureShoppingCart.Options;

public sealed class BlobStorageOptions
{
    public string ConnectionString { get; init; } = default!;
    public string Container { get; init; } = default!;
}
