using Microsoft.Extensions.Options;

namespace AzureShoppingCart.Options.Setups;

public sealed class BlobStorageSetup(IConfiguration configuration) 
    : IConfigureOptions<BlobStorageOptions>
{
    private const string SectionName = "Blob";

    public void Configure(BlobStorageOptions options)
    {
        configuration
            .GetSection($"Azure:Storage:{SectionName}")
            .Bind(options);
    }
}
