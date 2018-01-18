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

        public ActionResult Index()
        {
            var imageCollection = _storageService.GetImages();
            ViewBag.ImageCollection = imageCollection;

            return View();
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
                await _storageService.AddImage(file.InputStream, fileExtension);

                await _cognitiveService.UploadAndDetectFaces(file.InputStream);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { value = "uploadFailure" });
            }

            return RedirectToAction("Index", new { value = "uploadSuccess" });
        }
    }
}