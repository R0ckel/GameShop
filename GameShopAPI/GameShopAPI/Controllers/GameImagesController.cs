using GameShopAPI.DTOs.Game;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.IModelImageService;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameImagesController : ControllerBase
{
    private readonly IModelImageService<GameResponse> _gameImagesService;

    public GameImagesController(IModelImageService<GameResponse> gameImagesService)
    {
        _gameImagesService = gameImagesService;
    }

    // GET api/v1/GameImages/5/image
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, bool thumbnail = false)
    {
        var result = await _gameImagesService.GetImageAsync(id, thumbnail);

        if (result == null)
        {
            return NotFound(new BaseResponse<string>
            {
                Success = false,
                Message = "Image not found",
                StatusCode = StatusCodes.Status404NotFound
            });
        }

        return result;
    }

    // Post api/v1/GameImages/5/image
    [HttpPost("{id}")]
    public async Task<BaseResponse<GameResponse>> Post(Guid id, IFormFile file)
        => await _gameImagesService.UploadImageAsync(id, file);

    // Delete api/v1/GameImages/5/image
    [HttpDelete("{id}")]
    public async Task<BaseResponse<GameResponse>> Delete(Guid id)
        => await _gameImagesService.DeleteImageAsync(id);
}
