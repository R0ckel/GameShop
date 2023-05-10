using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.Basket;

public class RemoveBasketItemRequest
{
    [Required]
    public Guid GameId { get; set; }
}
