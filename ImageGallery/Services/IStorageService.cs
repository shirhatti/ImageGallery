using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageGallery.Models;
namespace ImageGallery.Services
{
    public interface IStorageService
    {
        IEnumerable<Image> GetImages();
        Task AddImage(Stream stream);
    }
}