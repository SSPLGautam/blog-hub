using BlogApp.Models;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Web.Helper
{
    public class ImagesHelper
    {
        private static readonly string[] _allowedExt = { ".jpg", ".png", ".jpeg", ".webp" };

        public static string UploadFile(IFormFile file)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string folderPath = Path.Combine("wwwroot", "images");   

            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExt.Contains(ext))
                throw new Exception("Invalid Image Format");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return "/images/" + fileName;
        }

        public static IFormFile? GetImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var absolutePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                filePath.TrimStart('/')
            );

            var fileInfo = new FileInfo(absolutePath);

            if (!fileInfo.Exists)
                return null;

            var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);

            var formFile = new FormFile(
                stream,
                0,
                fileInfo.Length,
                "file",
                fileInfo.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream",
                ContentDisposition = $"form-data; name=\"file\"; filename=\"{fileInfo.Name}\""
            };

            return formFile;
        }
    }
}