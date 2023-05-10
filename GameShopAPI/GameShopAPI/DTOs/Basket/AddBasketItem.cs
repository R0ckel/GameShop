using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.Basket;

public class AddBasketItemRequest
{
    [Required]
    public Guid GameId { get; set; }
}
