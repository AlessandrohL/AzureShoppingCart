namespace AzureShoppingCart.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType);
}
