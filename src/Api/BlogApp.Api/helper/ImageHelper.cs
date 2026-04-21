namespace BlogApp.Api.Helper;

public static class ImageHelper
{
    private static readonly string[] _allowedExt = { ".jpg", ".png", ".jpeg", ".webp" };

    public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLower();

        if (!_allowedExt.Contains(ext))
            throw new Exception("Invalid Image Format");

        var fileName = Guid.NewGuid() + ext;
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return "/images/" + fileName;
    }
}
