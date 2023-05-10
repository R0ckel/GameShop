using GameShopAPI.DTOs.UserRole;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.UserRoleService;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UserRoleController : ControllerBase
{
    private IUserRoleService _userRoleService;

    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    // GET: api/v1/UserRole + ?params
    [HttpGet]
    public async Task<BaseResponse<UserRoleResponse>> Get([FromQuery] UserRoleFilter? filter,
                                                         int page = 1,
                                                         int pageSize = 10)
        => await _userRoleService.ReadPageAsync(page, pageSize, filter);

    // GET api/v1/UserRole/5
    [HttpGet("{id}")]
    public async Task<BaseResponse<UserRoleResponse>> Get(int id)
        => await _userRoleService.ReadAsync(id);

    // POST api/v1/UserRole
    [HttpPost]
    public async Task<BaseResponse<UserRoleResponse>> Post([FromBody] CreateUserRoleRequest request)
        => await _userRoleService.CreateAsync(request);

    // PUT api/v1/UserRole/5
    [HttpPut("{id}")]
    public async Task<BaseResponse<UserRoleResponse>> Put(int id,
                                                         [FromBody] UpdateUserRoleRequest request)
        => await _userRoleService.UpdateAsync(id, request);

    // DELETE api/v1/UserRole/5
    [HttpDelete("{id}")]
    public async Task<BaseResponse<UserRoleResponse>> Delete(int id)
        => await _userRoleService.DeleteAsync(id);
}
