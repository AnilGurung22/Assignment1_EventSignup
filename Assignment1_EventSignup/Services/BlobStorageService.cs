using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Assignment1_EventSignup.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["BlobStorage:ConnectionString"]
                ?? throw new InvalidOperationException("BlobStorage:ConnectionString is not configured.");
            _containerName = configuration["BlobStorage:ContainerName"] ?? "event-banners";
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);

            // Make sure the container exists and allows public read access to blobs
            // so banner images can be displayed directly via their URL.
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var extension = Path.GetExtension(file.FileName);
            var blobName = $"{Guid.NewGuid()}{extension}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            }

            return blobClient.Uri.ToString();
        }
    }
}
