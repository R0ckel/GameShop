using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.Game;

public class CreateGameRequest
{
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(0, double.MaxValue)]
    public double Price { get; set; }

    [Required, MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    // Navigation
    [Required]
    public Guid CompanyId { get; set; }
    public ICollection<Guid> GenreIds { get; set; } = new HashSet<Guid>();
}
