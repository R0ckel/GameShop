using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.UserRole;

public class UpdateUserRoleRequest
{
    public int Id { get; set; }

    [Required, MaxLength(30)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }
}
