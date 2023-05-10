using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.GameGenre;

public class CreateGameGenreRequest
{
    [Required, MaxLength(30)]
    public string? Name { get; set; }

    [Required, MaxLength(255)]
    public string? Description { get; set; }
}
