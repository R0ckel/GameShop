using GameShopAPI.Data;
using GameShopAPI.DTOs.Game;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.IModelImageService;
using GameShopAPI.Services.ImageService;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Services.GameImagesService;

public class GameImageService : IModelImageService<GameResponse>
{
    private readonly GameShopContext _context;
    private readonly IImageService _imageService;

    public GameImageService(GameShopContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<FileContentResult?> GetImageAsync(Guid gameId, bool thumbnail = false)
    {
        try
        {
            var game = await _context.Games.FindAsync(gameId);

            if (game == null)
            {
                return null;
            }

            var path = thumbnail ? game.ThumbnailImagePath : game.ImagePath;

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

    public async Task<BaseResponse<GameResponse>> UploadImageAsync(Guid gameId, IFormFile imageFile)
    {
        var response = new BaseResponse<GameResponse>();

        try
        {
            var game = await _context.Games.FindAsync(gameId);

            if (game == null)
            {
                response.Message = "Game not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            // Delete current image if exists
            if (!string.IsNullOrWhiteSpace(game.ImagePath))
                _imageService.DeleteImage(game.ImagePath);
            if (!string.IsNullOrEmpty(game.ThumbnailImagePath))
                _imageService.DeleteImage(game.ThumbnailImagePath);

            var model = new SaveImageModel(game.Id.ToString(), "game", imageFile);

            // Save image (original + thumbnail)
            var saveImageResult = await _imageService.SaveImageAsync(model);
            if (!saveImageResult.Success)
            {
                response.Message = saveImageResult.ErrorMessage;
                return response;
            }
            game.ImagePath = saveImageResult.MainPath;
            game.ThumbnailImagePath = saveImageResult.ThumbnailPath;

            await _context.SaveChangesAsync();

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

    public async Task<BaseResponse<GameResponse>> DeleteImageAsync(Guid gameId)
    {
        var response = new BaseResponse<GameResponse>();

        try
        {
            var game = await _context.Games.FindAsync(gameId);

            if (game == null)
            {
                response.Message = "Game not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (!string.IsNullOrWhiteSpace(game.ImagePath))
                _imageService.DeleteImage(game.ImagePath);
            if (!string.IsNullOrEmpty(game.ThumbnailImagePath))
                _imageService.DeleteImage(game.ThumbnailImagePath);

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Image deleted successfully";
            response.StatusCode = StatusCodes.Status204NoContent;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}