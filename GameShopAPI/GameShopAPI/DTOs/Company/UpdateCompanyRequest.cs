using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.Company;

public class UpdateCompanyRequest
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string? Name { get; set; }

    [Required, MaxLength(255)]
    public string? Country { get; set; }
}
