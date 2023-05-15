using GameShopAPI.Services.IModelImageService;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Services.ImageService;

public interface IImageService
{
    Task<FileContentResult?> GetImageAsync(string path);
    Task<SaveImageResult> SaveImageAsync(SaveImageModel info);
    void DeleteImage(string imagePath);
    SaveImageResult SaveEmpty(string path);
}
