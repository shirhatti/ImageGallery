using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private readonly HttpClient _httpClient;
        private readonly string imagePrefix = "img_";
        private readonly string metaDataPrefix = "metadata_";

        public StorageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
        public async Task<Image> AddImageAsync(Stream stream, string fileExtension)
        {
            var imageGuid = Guid.NewGuid();
            var fileName = String.Concat(imagePrefix, imageGuid, ".", fileExtension);
            var imageBlob = _container.GetBlockBlobReference(fileName);
            await imageBlob.UploadFromStreamAsync(stream);

            return new Image()
            {
                ImageGuid = imageGuid,
                ImagePath = imageBlob.Uri
            };
        }

        public async Task AddMetadataAsync(Image image, Face[] faces)
        {
            var imageUri = image.ImagePath;
            // Associate the image URI with the metadata
            var fileName = String.Concat(metaDataPrefix, image.ImageGuid, ".json");
            var metadataBlob = _container.GetBlockBlobReference(fileName);

            var json = JsonConvert.SerializeObject(faces);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                await metadataBlob.UploadFromStreamAsync(stream);
            }
        }

        public async Task<IEnumerable<Image>> GetImagesAsync()
        {
            var imageList = new List<Image>();
            var blobList = _container.ListBlobs(imagePrefix);
            var metadataList = _container.ListBlobs(metaDataPrefix);

            foreach (var blob in blobList)
            {
                var image = new Image
                {
                    ImagePath = blob.Uri
                };
                image.ImageGuid = new Guid(image.ImagePath.AbsolutePath.Substring(image.ImagePath.AbsolutePath.IndexOf(imagePrefix) + imagePrefix.Length, Guid.Empty.ToString().Length));
                var metadataUri = String.Concat(_container.Uri, "/", metaDataPrefix, image.ImageGuid, ".json");
                await _httpClient.GetAsync(metadataUri)
                                                .ContinueWith(async (responseTask) =>
                {
                    var response = responseTask.Result;
                    var jsonString = await response.Content.ReadAsStringAsync();
                    image.FaceAttributes = JsonConvert.DeserializeObject<List<Face>>(jsonString);
                });

                imageList.Add(image);
            }

            return imageList;
        }
    }
}