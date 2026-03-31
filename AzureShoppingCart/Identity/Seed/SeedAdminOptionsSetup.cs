using Microsoft.Extensions.Options;

namespace AzureShoppingCart.Identity.Seed;

public sealed class SeedAdminOptionsSetup(IConfiguration configuration) 
    : IConfigureOptions<SeedAdminOptions>
{
    private const string RootName = "Seed";
    private const string SectionName = "DefaultAdmin";

    public void Configure(SeedAdminOptions options)
    {
        configuration
            .GetSection(RootName)
            .GetSection(SectionName)
            .Bind(options);

        if (string.IsNullOrWhiteSpace(options.Email))
            throw new InvalidOperationException("Seed admin email is not configured");

        if (string.IsNullOrWhiteSpace(options.Password))
            throw new InvalidOperationException("Seed admin password is not configured");
    }
}
