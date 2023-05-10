using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.User;

public class RegisterUserRequest
{
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string Surname { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;


    [Required]
    public DateOnly BirthDate { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;
}
