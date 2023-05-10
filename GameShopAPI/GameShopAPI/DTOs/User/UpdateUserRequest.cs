using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.User;

public class UpdateUserRequest
{
    public Guid Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Surname { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}
