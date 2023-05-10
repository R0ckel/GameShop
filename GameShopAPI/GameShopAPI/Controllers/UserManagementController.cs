using GameShopAPI.DTOs.User;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.AuthService;
using GameShopAPI.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UserManagementController : ControllerBase
{
    private IUserService _userService;
    private IAuthService _authService;

    public UserManagementController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet("{id}")]
    public async Task<BaseResponse<UserResponse>> Get(Guid id) 
        => await _userService.ReadAsync(id);

    [HttpPost("register")]
    public async Task<BaseResponse<UserResponse>> Register(RegisterUserRequest request)
        => await _userService.CreateAsync(request);

    [HttpPost("login")]
    public async Task<BaseResponse<AuthResponse>> Login(LoginUserRequest request)
        => await _authService.Login(request);

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateUserRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != id.ToString())
        {
            return Forbid();
        }

        var response = await _userService.UpdateAsync(id, request);
        return Ok(response);
    }
}
