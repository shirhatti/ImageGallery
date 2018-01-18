using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Configuration;
using ImageGallery.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageGallery.Services
{
    public class StorageService : IStorageService
    {
        CloudStorageAccount storageAccount;
        CloudBlobClient blobClient;
        CloudBlobContainer imageContainer;
        private readonly string imagePrefix = "img_";
        private readonly string metaDataPrefix = "metadata_";

        public StorageService()
        {
            var connectionString = WebConfigurationManager.AppSettings["AzureStorageConnectionString"];
            storageAccount = CloudStorageAccount.Parse(connectionString);
            blobClient = storageAccount.CreateCloudBlobClient();
            imageContainer = blobClient.GetContainerReference("images");
            imageContainer.CreateIfNotExists();
            var permissions = imageContainer.GetPermissions();
            if (permissions.PublicAccess == BlobContainerPublicAccessType.Off || permissions.PublicAccess == BlobContainerPublicAccessType.Unknown)
            {
                // If blob isn't public, we can't directly link to the pictures
                imageContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
        }
        public async Task AddImage(Stream stream, string fileExtension)
        {
            var fileName = String.Concat(imagePrefix, Guid.NewGuid(), ".", fileExtension);
            var imageBlob = imageContainer.GetBlockBlobReference(fileName);
            await imageBlob.UploadFromStreamAsync(stream);
        }

        public IEnumerable<Image> GetImages()
        {
            var imageList = new List<Image>();
            var blobList = imageContainer.ListBlobs(imagePrefix);
            foreach (var blob in blobList)
            {
                var image = new Image
                {
                    ImagePath = blob.StorageUri.PrimaryUri
                };
                imageList.Add(image);
            }
            return imageList;
        }
    }
}