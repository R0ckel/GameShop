using GameShopAPI.DTOs.GameGenre;

namespace GameShopAPI.DTOs.Game;

public class GameResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public List<GameGenreCard> Genres { get; set; } = new List<GameGenreCard>();
}
