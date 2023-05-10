using GameShopAPI.Data;
using GameShopAPI.DTOs.User;
using GameShopAPI.Services.IModelImageService;
using GameShopAPI.Services.ImageService;
using Microsoft.AspNetCore.Mvc;
using GameShopAPI.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Services.UserImageService;

public class UserImageService : IModelImageService<UserResponse>
{
    private readonly GameShopContext _context;
    private readonly IImageService _imageService;

    public UserImageService(GameShopContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<FileContentResult?> GetImageAsync(Guid userId, bool thumbnail = false)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return null;
            }

            var path = thumbnail ? user.ThumbnailImagePath : user.ImagePath;

            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var image = await _imageService.GetImageAsync(path);

            return image;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<BaseResponse<UserResponse>> UploadImageAsync(Guid userId, IFormFile imageFile)
    {
        var response = new BaseResponse<UserResponse>();

        try
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                response.Message = "User not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            var currentImagePath = user.ImagePath;
            var currentThumbnailImagePath = user.ThumbnailImagePath;

            var model = new SaveImageModel(user.Id.ToString(), "user", imageFile);

            // Disable the trigger
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Users DISABLE TRIGGER SetUserLocked");

            // Save image (original + thumbnail)
            var saveImageResult = await _imageService.SaveImageAsync(model);
            if (!saveImageResult.Success)
            {
                response.Message = saveImageResult.ErrorMessage;
                return response;
            }
            user.ImagePath = saveImageResult.MainPath;
            user.ThumbnailImagePath = saveImageResult.ThumbnailPath;

            await _context.SaveChangesAsync();

            // Re-enable the trigger
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Users ENABLE TRIGGER SetUserLocked");

            response.Success = true;
            response.Message = "Image uploaded successfully";
            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<UserResponse>> DeleteImageAsync(Guid userId)
    {
        var response = new BaseResponse<UserResponse>();

        try
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                response.Message = "User not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            // Disable the trigger
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Users DISABLE TRIGGER SetUserLocked");

            if (!string.IsNullOrWhiteSpace(user.ImagePath))
                _imageService.DeleteImage(user.ImagePath);
            if (!string.IsNullOrEmpty(user.ThumbnailImagePath))
                _imageService.DeleteImage(user.ThumbnailImagePath);

            await _context.SaveChangesAsync();

            // Re-enable the trigger
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Users ENABLE TRIGGER SetUserLocked");

            response.Success = true;
            response.Message = "Image deleted successfully";
            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
