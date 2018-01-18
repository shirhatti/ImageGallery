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
        private IStorageService _service;
        public HomeController(IStorageService service)
        {
            _service = service;
        }
        public ActionResult Index()
        {
            var imageCollection = _service.GetImages();
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
                await _service.AddImage(file.InputStream, fileExtension);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", new { value = "uploadFailure" });
            }

            return RedirectToAction("Index", new { value = "uploadSuccess" });
        }
    }
}