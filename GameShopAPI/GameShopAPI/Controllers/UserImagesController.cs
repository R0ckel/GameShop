using GameShopAPI.DTOs.User;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.IModelImageService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UserImagesController : ControllerBase
{
    private readonly IModelImageService<UserResponse> _userImagesService;

    public UserImagesController(IModelImageService<UserResponse> userImagesService)
    {
        _userImagesService = userImagesService;
    }

    // GET api/v1/UserImages/5/image
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, bool thumbnail = false)
    {
        var result = await _userImagesService.GetImageAsync(id, thumbnail);

        if (result == null)
        {
            return NotFound(new BaseResponse<string>
            {
                Message = "Image not found",
                StatusCode = StatusCodes.Status404NotFound
            });
        }

        return result;
    }

    // Put api/v1/UserImages/5/image
    [HttpPut("{id}")]
    public async Task<BaseResponse<UserResponse>> Put(Guid id, IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != id.ToString())
        {
            return new BaseResponse<UserResponse>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = "Forbidden to put image of another account"
            };
        }

        return await _userImagesService.UploadImageAsync(id, file);
    }

    // Delete api/v1/UserImages/5/image
    [HttpDelete("{id}")]
    public async Task<BaseResponse<UserResponse>> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != id.ToString())
        {
            return new BaseResponse<UserResponse>
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = "Forbidden to delete image of another account"
            };
        }

        return await _userImagesService.DeleteImageAsync(id);
    }
}
