using GameShopAPI.DTOs.User;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.AuthService;

public interface IAuthService
{
    Task<BaseResponse<AuthResponse>> Login(LoginUserRequest request);
}
