using Microsoft.Extensions.Options;

namespace AzureShoppingCart.Options.Setups;

public sealed class JwtSetup(IConfiguration configuration)
    : IConfigureOptions<JwtOptions>
{
    private const string SectionName = "Jwt";

    public void Configure(JwtOptions options)
    {
        configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
