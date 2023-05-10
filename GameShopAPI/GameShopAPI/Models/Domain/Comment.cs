using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.Models.Domain;

public class Comment
{
    public Guid Id { get; set; }

    [Required, MaxLength(3000)]
    public string? Text { get; set; }
    public DateTime Created { get; set; }

    // Navigation
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid GameId { get; set; }
    public Game? Game { get; set; }
}
