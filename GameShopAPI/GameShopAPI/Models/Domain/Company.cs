using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.Models.Domain;

public class Company
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string? Name { get; set; }

    [Required, MaxLength(255)]
    public string? Country { get; set; }

    // Navigation
    public ICollection<Game> Games { get; set; } = new List<Game>();
}