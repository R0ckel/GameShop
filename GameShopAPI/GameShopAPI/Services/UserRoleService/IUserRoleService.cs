using GameShopAPI.DTOs.UserRole;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.UserRoleService;

public interface IUserRoleService
{
    Task<BaseResponse<UserRoleResponse>> CreateAsync(CreateUserRoleRequest request);
    Task<BaseResponse<UserRoleResponse>> ReadAsync(int id);
    Task<BaseResponse<UserRoleResponse>> UpdateAsync(int id, UpdateUserRoleRequest request);
    Task<BaseResponse<UserRoleResponse>> DeleteAsync(int id);
    Task<BaseResponse<UserRoleResponse>> ReadPageAsync(int page, int pageSize, UserRoleFilter? filter);
}
