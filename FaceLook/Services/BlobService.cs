using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace FaceLook.Services
{
    public class BlobService(IOptions<BlobStorageOptions> blobOptions, IUserService userService) : IBlobService
    {
        private readonly BlobContainerClient blobContainerClient = GetBlobContainerClient(blobOptions.Value);

        public async Task UploadBlob(IFormFile picture)
        {
            var blobClient = await GetBlobClient(Guid.NewGuid().ToString() + picture.FileName, true);
            var options = new BlobHttpHeaders()
            {
                ContentType = picture.ContentType,
            };
            await blobClient.UploadAsync(picture.OpenReadStream(), options);
        }

        private static BlobContainerClient GetBlobContainerClient(BlobStorageOptions blobOptions)
        {
            BlobServiceClient blobServiceClient = new(blobOptions.ConnectionString);
            return blobServiceClient.GetBlobContainerClient(blobOptions.ContainerName);
        }

        private async Task<BlobClient> GetBlobClient(string blobName, bool generateBlobName = false)
        {
            string clientBlobName = generateBlobName ? await GenerateUserBlobName(blobName) : blobName;
            return blobContainerClient.GetBlobClient(clientBlobName);
        }

        private async Task<string> GenerateUserBlobName(string? blobName = null)
        {
            string userName = await userService.GetRequiredCurrentUserNameAsync();

            return Path.Combine(userName, blobName ?? string.Empty);
        }
    }
}
