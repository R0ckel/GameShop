using GameShopAPI.DTOs.Basket;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.BasketService;

public interface IBasketService
{
    Task<BaseResponse<BasketItemResponse>> ReadAsync(Guid userId);
    Task<BaseResponse<BasketItemResponse>> AddAsync(AddBasketItemRequest request, Guid userId);
    Task<BaseResponse<BasketItemResponse>> RemoveAsync(RemoveBasketItemRequest request, Guid userId);
    Task<BaseResponse<BasketItemResponse>> RemoveAllAsync(Guid userId);
    Task<BaseResponse<decimal>> GetBasketTotalPrice(Guid userId);
}