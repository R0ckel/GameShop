namespace GameShopAPI.Models.Domain;

public class BasketItem
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid GameId { get; set; }
    public Game? Game { get; set; }
}
