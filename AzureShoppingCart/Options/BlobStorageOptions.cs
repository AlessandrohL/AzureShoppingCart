namespace AzureShoppingCart.Options;

public sealed class BlobStorageOptions
{
    public string Uri { get; init; } = default!;
    public string Container { get; init; } = default!;
}
