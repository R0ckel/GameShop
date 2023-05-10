namespace GameShopAPI.DTOs.Basket;

public class BasketItemResponse
{
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public double GamePrice { get; set; }
}
