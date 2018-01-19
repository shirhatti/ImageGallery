using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageGallery.Models;
using Microsoft.ProjectOxford.Face.Contract;

namespace ImageGallery.Services
{
    public interface IStorageService
    {
        IEnumerable<Image> GetImages();
        Task<Uri> AddImageAsync(Stream stream, string fileExtension);
        Task AddMetadataAsync(Uri imageUri, Face[] faces);
    }
}