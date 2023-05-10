using GameShopAPI.DTOs.Basket;
using GameShopAPI.Models.Domain;

namespace GameShopAPI.Extensions.Domain;

public static class BasketExtensions
{
    public static BasketItem ToModel(this AddBasketItemRequest request)
    {
        return new BasketItem
        {
            GameId = request.GameId
        };
    }

    public static BasketItemResponse ToResponse(this BasketItem item)
    {
        return new BasketItemResponse
        {
            GameId = item.GameId,
            GameName = item.Game?.Name ?? string.Empty,
            GamePrice = item.Game?.Price ?? -1
        };
    }
}
