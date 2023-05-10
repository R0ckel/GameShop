using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.UserRole;

public class CreateUserRoleRequest
{
    [Required, MaxLength(30)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }
}
