using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using LomographyStoreApi.Services.Interfaces;

namespace LomographyStoreApi.Services
{
    public class AzureBlobService : IBlobService
    {
        private readonly IConfiguration _config;
        private readonly BlobContainerClient _containerClient;

        public AzureBlobService(IConfiguration configuration)
        {
            _config = configuration;

            var connectionStr = _config[Constants.KEY_STORAGE_CNN];
            var containerName = _config[Constants.KEY_BLOB];
            _containerClient = new BlobContainerClient(connectionStr, containerName);
        }

        public async Task<string> UploadBlobAsync(string blobName, Stream imageStream)
        {
             //Required if Azure Resource Manager was used to create infrastucture
            await _containerClient.CreateIfNotExistsAsync();

            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(imageStream,
                new BlobHttpHeaders
                {
                    ContentType = "image/jpeg",
                    CacheControl = "public"
                });

            return blobClient.Uri.ToString();
        }
    }
}