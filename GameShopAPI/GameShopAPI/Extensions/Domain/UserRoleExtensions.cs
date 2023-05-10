using GameShopAPI.DTOs.UserRole;
using GameShopAPI.Models.Domain;

namespace GameShopAPI.Extensions.Domain;

public static class UserRoleExtensions
{
    public static UserRole ToModel(this CreateUserRoleRequest request)
    {
        return new UserRole
        {
            Name = request.Name,
            Description = request.Description
        };
    }

    public static UserRole ToModel(this UpdateUserRoleRequest request)
    {
        return new UserRole
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description
        };
    }

    public static UserRoleResponse ToResponse(this UserRole genre)
    {
        return new UserRoleResponse
        {
            Id = genre.Id,
            Name = genre.Name ?? string.Empty,
            Description = genre.Description ?? string.Empty
        };
    }

    public static UserRoleCard ToCard(this UserRole genre)
    {
        return new UserRoleCard
        {
            Id = genre.Id,
            Name = genre.Name ?? string.Empty
        };
    }
}
