using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.Models.Domain;

public class Game
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string? Name { get; set; }

    [Required, Range(0, double.MaxValue)]
    public double Price { get; set; }

    [Required, MaxLength(5000)]
    public string? Description { get; set; }

    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }

    // Navigation
    [Required]
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    public ICollection<GameGenre> Genres { get; set; } = new List<GameGenre>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
}
