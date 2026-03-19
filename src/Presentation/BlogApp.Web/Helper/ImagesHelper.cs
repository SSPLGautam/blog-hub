
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

        public static void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var absolutePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                filePath.TrimStart('/')
            );
            try
            {
                if (File.Exists(absolutePath))
                {
                    File.SetAttributes(absolutePath, FileAttributes.Normal);
                    File.Delete(absolutePath);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("File delete error: " + ex.Message);

            }
        }
   
        public static string GetImagePath(IFormFile file)
        {
            return $@"/images/{file.FileName}";
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

            if (!File.Exists(absolutePath))
                return null;

            byte[] fileBytes = File.ReadAllBytes(absolutePath);

            var stream = new MemoryStream(fileBytes);

            return new FormFile(
                stream,
                0,
                fileBytes.Length,
                "file",
                Path.GetFileName(absolutePath)
            );
        }
    }
}