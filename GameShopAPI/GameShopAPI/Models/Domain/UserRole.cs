using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.Models.Domain;

public class UserRole
{
    public int Id { get; set; }

    [Required, MaxLength(30)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
