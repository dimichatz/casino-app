using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace CasinoAppBackend.Services.FileStorage
{
    /// <summary>
    /// File storage service backed by Azure Blob Storage.  
    /// Supports uploading, deleting, and generating time-limited SAS URLs for blobs.  
    /// Automatically ensures the target container exists during initialization.
    /// </summary>
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(IConfiguration config)
        {
            var connectionString = config["AzureStorage:ConnectionString"];
            var containerName = config["AzureStorage:ContainerName"];

            _containerClient = new BlobContainerClient(connectionString, containerName);

            // Ensure container exists (safe if already exists)
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            // Create the blob reference
            var blobClient = _containerClient.GetBlobClient(fileName);

            // Delete any existing blob (this is safe for overwrite logic)
            await blobClient.DeleteIfExistsAsync();

            // Upload the new blob with headers
            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                }
            );

            return fileName; // Return the unique blob name
        }

        public async Task DeleteAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public string GetSasUrl(string blobName, int minutesValid = 10)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri)
                throw new InvalidOperationException("SAS generation is not permitted. Ensure your connection string includes account key.");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerClient.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(minutesValid)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }
    }
}