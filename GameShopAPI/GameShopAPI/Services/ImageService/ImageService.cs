using GameShopAPI.Services.IModelImageService;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace GameShopAPI.Services.ImageService;

public class ImageService : IImageService
{
    public async Task<FileContentResult?> GetImageAsync(string path)
    {
        var imageData = await File.ReadAllBytesAsync(path);
        return new FileContentResult(imageData, "image/jpeg");
    }

    public SaveImageResult SaveEmpty(string path)
    {
        var result = new SaveImageResult();
        try
        {
            var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }
            var filePath = Path.Combine("Images", $"{path}.jpg");

            File.Create(filePath).Close();

            result.Success = true;
            result.MainPath = filePath;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }
        return result;
    }

    public async Task<SaveImageResult> SaveImageAsync(SaveImageModel info)
    {
        var result = new SaveImageResult();
        try
        {
            var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            result.MainPath = await SaveMainImageAsync(info);
            if (string.IsNullOrWhiteSpace(result.MainPath))
            {
                result.ErrorMessage = "Error while saving original image";
                return result;
            }

            result.ThumbnailPath = await SaveThumbnailImageAsync(info);
            if (string.IsNullOrWhiteSpace(result.ThumbnailPath))
            {
                DeleteImage(result.MainPath);
                result.ErrorMessage = "Error while saving thumbnail image";
                return result;
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }
        return result;
    }

    public void DeleteImage(string imagePath)
    {
        if (File.Exists(imagePath))
            File.Delete(imagePath);
    }

    private async Task<string> SaveMainImageAsync(SaveImageModel model)
    {
        if (model.File == null)
        {
            return string.Empty;
        }

        var imagePath = Path.Combine("Images", 
            $"{model.Prefix}_{model.Name}.jpg");

        using (var fileStream = new FileStream(imagePath, FileMode.Create))
        {
            await model.File.CopyToAsync(fileStream);
        }

        return imagePath;
    }

    private async Task<string> SaveThumbnailImageAsync(SaveImageModel model)
    {
        if (model.File == null)
        {
            return string.Empty;
        }

        var thumbnailImagePath = Path.Combine("Images", $"{model.Prefix}_{model.Name}_thumbnail.jpg");

        using (var fileStream = new FileStream(thumbnailImagePath, FileMode.Create))
        {
            using var image = Image.Load(model.File.OpenReadStream());

            // Calculate the size of the square to crop
            var minDimension = Math.Min(image.Width, image.Height);
            var x = (image.Width - minDimension) / 2;
            var y = (image.Height - minDimension) / 2;

            // Crop the image to a square
            image.Mutate(context => context.Crop(new Rectangle(x, y, minDimension, minDimension)));

            // Resize the image
            image.Mutate(x => x.Resize(128, 128));

            await image.SaveAsync(fileStream, new JpegEncoder());
        }

        return thumbnailImagePath;
    }
}
