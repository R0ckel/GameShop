using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.Models.Domain;

public class User
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string? Name { get; set; }

    [Required, MaxLength(255)]
    public string? Surname { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime BirthDate { get; set; }

    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    // Avatar
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }

    // Lock security
    public int AuthFailedCount { get; set; } = 0;
    public bool IsLocked { get; set; } = false;

    // Navigation
    public int RoleId { get; set; } = 1;
    public UserRole? Role { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
}
