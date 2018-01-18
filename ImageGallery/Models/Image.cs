using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageGallery.Models
{
    public class Image
    {
        public Uri ImagePath { get; set; }
        public Uri ImageMetaDataPath { get; set; }
    }
}