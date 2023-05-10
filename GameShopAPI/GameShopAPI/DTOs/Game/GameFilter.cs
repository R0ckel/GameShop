namespace GameShopAPI.DTOs.Game;

public class GameFilter
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? PriceFrom { get; set; }
    public double? PriceTo { get; set; }
    public string? CompanyName { get; set; }
    public List<Guid>? GenreIds { get; set; }
}
