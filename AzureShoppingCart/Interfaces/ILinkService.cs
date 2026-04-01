using AzureShoppingCart.Common;

namespace AzureShoppingCart.Interfaces;

public interface ILinkService
{
    public Link Generate(string endpointName, object? routeValues, string rel, string method);
}
