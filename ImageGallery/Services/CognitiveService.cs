using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace ImageGallery.Services
{
    public class CognitiveService : ICognitiveService
    {
        private readonly IFaceServiceClient _client;

        public CognitiveService()
        {
            var url = WebConfigurationManager.AppSettings["CognitiveServiciesUrl"];
            var key = WebConfigurationManager.AppSettings["CognitiveServicesFaceApiKey"];

            _client = new FaceServiceClient(key, url);
        }

        public async Task<Face[]> DetectFacesAsync(Stream stream)
        {
            var attributes = new FaceAttributeType[]
            {
                FaceAttributeType.Gender,
                FaceAttributeType.Age,
                FaceAttributeType.Smile,
                FaceAttributeType.Emotion,
                FaceAttributeType.Glasses,
                FaceAttributeType.Hair
            };
            return await _client.DetectAsync(stream, returnFaceId: true, returnFaceLandmarks: true, returnFaceAttributes: attributes);
        }

        public async Task<Face[]> UploadAndDetectFaces(Uri imageUri)
        {
            var attributes = new FaceAttributeType[]
            {
                FaceAttributeType.Gender,
                FaceAttributeType.Age,
                FaceAttributeType.Smile,
                FaceAttributeType.Emotion,
                FaceAttributeType.Glasses,
                FaceAttributeType.Hair
            };

            return await _client.DetectAsync(imageUri.ToString(), returnFaceId: true, returnFaceLandmarks: true, returnFaceAttributes: attributes);
        }
    }
}