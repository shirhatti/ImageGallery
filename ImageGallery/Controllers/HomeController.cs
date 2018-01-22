using ImageGallery.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ImageGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStorageService _storageService;
        private readonly ICognitiveService _cognitiveService;

        public HomeController(IStorageService storageService, ICognitiveService cognitiveService)
        {
            _storageService = storageService;
            _cognitiveService = cognitiveService;
        }

        public async Task<ActionResult> Index()
        {
            var images = await _storageService.GetImagesAsync();
            return View(images);
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return RedirectToAction("Index", new { value = "uploadFailure" });
            }

            try
            {
                var fileExtension = Path.GetExtension(file.FileName);

                var image = await _storageService.AddImageAsync(file.InputStream, fileExtension);

                var faces = await _cognitiveService.UploadAndDetectFaces(image.ImagePath);

                await _storageService.AddMetadataAsync(image, faces);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", new { value = "uploadFailure" });
            }
            
            return RedirectToAction("Index", new { value = "uploadSuccess" });
        }
    }
}