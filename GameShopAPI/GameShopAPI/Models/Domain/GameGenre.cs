using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.Models.Domain;

public class GameGenre
{
    public Guid Id { get; set; }

    [Required, MaxLength(30)]
    public string? Name { get; set; }

    [Required, MaxLength(255)]
    public string? Description { get; set; }

    // Navigation
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
