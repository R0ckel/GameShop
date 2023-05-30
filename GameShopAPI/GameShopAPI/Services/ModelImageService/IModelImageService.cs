using GameShopAPI.Models.Base;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Services.IModelImageService;

public interface IModelImageService<T>
{
    Task<FileContentResult?> GetImageAsync(Guid gameId, bool thumbnail = false);
    Task<BaseResponse<T>> UploadImageAsync(Guid id, IFormFile imageFile);
    Task<BaseResponse<T>> DeleteImageAsync(Guid id);
}
