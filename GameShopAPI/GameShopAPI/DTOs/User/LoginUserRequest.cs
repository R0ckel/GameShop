using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.User;

public class LoginUserRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required] 
    public string Password { get; set; } = string.Empty;
}
