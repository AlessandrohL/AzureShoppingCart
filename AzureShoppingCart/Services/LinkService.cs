using AzureShoppingCart.Common;
using AzureShoppingCart.Interfaces;

namespace AzureShoppingCart.Services;

public sealed class LinkService(
    LinkGenerator linkGenerator,
    IHttpContextAccessor httpContextAccessor)
    : ILinkService
{
    public Link Generate(string endpointName, object? routeValues, string rel, string method)
    {
        string? href = linkGenerator.GetUriByName(
            httpContextAccessor.HttpContext!,
            endpointName,
            routeValues);

        return new Link
        {
            Href = href ?? throw new Exception("Invalid endpoint name provided"),
            Rel = rel,
            Method = method
        };
    }
}
