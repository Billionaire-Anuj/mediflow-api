using Mediflow.Helper;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Http;
using Mediflow.Domain.Common.Enum;
using Microsoft.AspNetCore.Hosting;
using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class FileService(IWebHostEnvironment webHostEnvironment) : IFileService
{
    public AssetDto UploadDocument(IFormFile file, string uploadedFilePath, string? prefix = null)
    {
        if (!Directory.Exists(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath)))
        {
            Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath));
        }

        var uploadedDocumentPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        var extension = Path.GetExtension(file.FileName);
        
        var fileName = UploadFile(file, uploadedDocumentPath, extension, prefix);

        var asset = new AssetDto
        {
            OriginalFileName = file.FileName,
            FileUrl = fileName
        };

        if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && extension != ".svg")
        {
            var originalPath = Path.Combine(uploadedDocumentPath, fileName);

            using var image = Image.Load(originalPath);

            var aspect = (double)image.Width / image.Height;

            var orientation = 
                aspect > 1.2 
                    ? Orientation.Wide 
                    : aspect < 0.8 
                        ? Orientation.Tall 
                        : Orientation.Square;

            asset.AspectRatio = aspect;
            asset.Orientation = orientation;
        }
        
        return asset;
    }

    public string UploadDocument(string base64Image, string uploadedFilePath, string? prefix = null)
    {
        return string.Empty;
    }
    
    public void DeleteFile(string uploadedFilePath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public void DeleteFolder(string folderPath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, folderPath);

        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }
    }
    
    public string FileExistPath(string uploadedFilePath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        return File.Exists(fullPath) ? fullPath : "";
    }
    
    private static string UploadFile(IFormFile file, string uploadedFilePath, string extension, string? prefix = null)
    {
        var fileName = string.IsNullOrEmpty(prefix) ? extension.SetUniqueFileName() : $"{prefix} - {extension.SetUniqueFileName()}";

        using var stream = new FileStream(Path.Combine(uploadedFilePath, fileName), FileMode.Create);

        file.CopyTo(stream);

        return fileName;
    }
}