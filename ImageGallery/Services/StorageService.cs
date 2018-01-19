using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using ImageGallery.Models;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace ImageGallery.Services
{
    public class StorageService : IStorageService
    {
        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _client;
        private readonly CloudBlobContainer _container;
        private readonly string imagePrefix = "img_";
        private readonly string metaDataPrefix = "metadata_";

        public StorageService()
        {
            var connectionString = WebConfigurationManager.AppSettings["AzureStorageConnectionString"];

            _account = CloudStorageAccount.Parse(connectionString);
            _client = _account.CreateCloudBlobClient();
            _container = _client.GetContainerReference("images");
            _container.CreateIfNotExists();

            var permissions = _container.GetPermissions();
            if (permissions.PublicAccess == BlobContainerPublicAccessType.Off || permissions.PublicAccess == BlobContainerPublicAccessType.Unknown)
            {
                // If blob isn't public, we can't directly link to the pictures
                _container.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
        }
        public async Task<Uri> AddImageAsync(Stream stream, string fileExtension)
        {
            var fileName = String.Concat(imagePrefix, Guid.NewGuid(), ".", fileExtension);
            var imageBlob = _container.GetBlockBlobReference(fileName);
            await imageBlob.UploadFromStreamAsync(stream);

            return imageBlob.Uri;
        }

        public async Task AddMetadataAsync(Uri imageUri, Face[] faces)
        {
            // Associate the image URI with the metadata
            var fileName = String.Concat(metaDataPrefix, imageUri, ".", Guid.NewGuid());
            var metadataBlob = _container.GetBlockBlobReference(fileName);

            var json = JsonConvert.SerializeObject(faces);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                await metadataBlob.UploadFromStreamAsync(stream);
            }
        }

        public IEnumerable<Image> GetImages()
        {
            var imageList = new List<Image>();
            var blobList = _container.ListBlobs(imagePrefix);
            var metadataList = _container.ListBlobs(metaDataPrefix);

            bool ContainsUri(Uri left, Uri right) =>
                left.ToString().Contains(right.ToString());

            foreach (var blob in blobList)
            {
                var image = new Image
                {
                    ImagePath = blob.Uri,
                    ImageMetaDataPath = metadataList.SingleOrDefault(mbLob => ContainsUri(mbLob.Uri, blob.Uri))?.Uri
                };

                imageList.Add(image);
            }

            return imageList;
        }
    }
}