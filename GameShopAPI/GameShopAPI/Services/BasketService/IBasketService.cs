using GameShopAPI.DTOs.Basket;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.BasketService;

public interface IBasketService
{
    Task<BaseResponse<BasketItemResponse>> ReadAsync(Guid userId);
    Task<BaseResponse<BasketItemResponse>> AddAsync(Guid gameId, Guid userId);
    Task<BaseResponse<BasketItemResponse>> RemoveAsync(Guid gameId, Guid userId);
    Task<BaseResponse<BasketItemResponse>> RemoveAllAsync(Guid userId);
    Task<BaseResponse<decimal>> GetBasketTotalPrice(Guid userId);
}