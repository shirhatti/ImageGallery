using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public interface ICognitiveService
    {
        Task<Face[]> UploadAndDetectFaces(Uri imageUri);
    }
}
