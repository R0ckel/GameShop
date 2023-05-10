using GameShopAPI.DTOs.User;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.UserService;

public interface IUserService
{
    Task<BaseResponse<UserResponse>> CreateAsync(RegisterUserRequest request);
    Task<BaseResponse<UserResponse>> ReadAsync(Guid id);
    Task<BaseResponse<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request);
}
