using GameShopAPI.Models.Domain;
using GameShopAPI.DTOs.User;

namespace GameShopAPI.Extensions.Domain;

public static class UserExtensions
{
    public static User ToModel(this RegisterUserRequest request)
    {
        return new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            BirthDate = request.BirthDate.ToDateTime(TimeOnly.MinValue)
        };
    }

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name ?? string.Empty,
            Surname = user.Surname ?? string.Empty,
            Email = user.Email ?? string.Empty,
            BirthDate = DateOnly.FromDateTime(user.BirthDate)
        };
    }
}
