using GameShopAPI.DTOs.Basket;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.BasketService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private IBasketService _commentService;

    public BasketController(IBasketService commentService)
    {
        _commentService = commentService;
    }

    // GET api/v1/Basket
    [HttpGet]
    public async Task<BaseResponse<BasketItemResponse>> GetBasket()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<BasketItemResponse>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.ReadAsync(userId);
    }

    // POST api/v1/Basket/add
    [HttpPost("add")]
    public async Task<BaseResponse<BasketItemResponse>> AddItem([FromBody] AddBasketItemRequest request)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<BasketItemResponse>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.AddAsync(request, userId);
    }

    // DELETE api/v1/Basket/remove
    [HttpDelete("remove")]
    public async Task<BaseResponse<BasketItemResponse>> Delete([FromBody] RemoveBasketItemRequest request)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<BasketItemResponse>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.RemoveAsync(request, userId);
    }

    // DELETE api/v1/Basket/clear
    [HttpDelete("clear")]
    public async Task<BaseResponse<BasketItemResponse>> ClearBasket()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<BasketItemResponse>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.RemoveAllAsync(userId);
    }

    // GET api/v1/Basket/total
    [HttpGet("total")]
    public async Task<BaseResponse<decimal>> GetBasketTotal()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<decimal>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.GetBasketTotalPrice(userId);
    }
}
