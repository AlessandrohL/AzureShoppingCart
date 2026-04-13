using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureShoppingCart.Interfaces;
using AzureShoppingCart.Options;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace AzureShoppingCart.Services;

public sealed class ImageStorageService : IImageStorageService
{
    private readonly BlobContainerClient _container;

    public ImageStorageService(
        IAzureClientFactory<BlobServiceClient> clientFactory,
        IOptions<BlobStorageOptions> options)
    {
        BlobServiceClient serviceClient = clientFactory.CreateClient("Images");
        string containerName = options.Value.Container;

        _container = serviceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType)
    {
        string newFileName = GenerateUniqueFileName(fileName);

        BlobClient blobClient = _container.GetBlobClient(newFileName);

        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });

        return blobClient.Uri.ToString();
    }

    private static string GenerateUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        var uniqueId = Guid.NewGuid();

        return $"{nameWithoutExtension}_{uniqueId}{extension}";
    }
}
