using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AzureShoppingCart.Extensions;

public static class CacheExtensions
{
    private static readonly DistributedCacheEntryOptions DefaultCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public static async Task<T?> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<CancellationToken, Task<T>> factory,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var cachedValue = await cache.GetStringAsync(key, cancellationToken);

        T? value;
        if (!string.IsNullOrEmpty(cachedValue))
        {
            value = JsonSerializer.Deserialize<T>(cachedValue);

            if (value is not null)
            {
                return value;
            }
        }

        value = await factory(cancellationToken);

        if (value is null)
        {
            return default;
        }

        await cache.SetStringAsync(key, JsonSerializer.Serialize(value), options ?? DefaultCacheOptions, cancellationToken);

        return value;
    }
}
