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
    private IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
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
        return await _basketService.ReadAsync(userId);
    }

    // POST api/v1/Basket/add
    [HttpPost("add/{id}")]
    public async Task<BaseResponse<BasketItemResponse>> AddItem(Guid id)
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
        return await _basketService.AddAsync(id, userId);
    }

    // DELETE api/v1/Basket/remove
    [HttpDelete("remove/{id}")]
    public async Task<BaseResponse<BasketItemResponse>> Delete(Guid id)
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
        return await _basketService.RemoveAsync(id, userId);
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
        return await _basketService.RemoveAllAsync(userId);
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
        return await _basketService.GetBasketTotalPrice(userId);
    }
}
