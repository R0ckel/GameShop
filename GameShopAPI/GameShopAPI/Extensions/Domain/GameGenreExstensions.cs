using GameShopAPI.DTOs.GameGenre;
using GameShopAPI.Models.Domain;

namespace GameShopAPI.Extensions.Domain;

public static class GameGenreExstensions
{
    public static GameGenre ToModel(this CreateGameGenreRequest request)
    {
        return new GameGenre
        {
            Name = request.Name,
            Description = request.Description
        };
    }

    public static GameGenre ToModel(this UpdateGameGenreRequest request)
    {
        return new GameGenre
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description
        };
    }

    public static GameGenreResponse ToResponse(this GameGenre genre)
    {
        return new GameGenreResponse
        {
            Id = genre.Id,
            Name = genre.Name,
            Description = genre.Description
        };
    }

    public static GameGenreCard ToCard(this GameGenre genre)
    {
        return new GameGenreCard
        {
            Id = genre.Id,
            Name = genre.Name ?? string.Empty
        };
    }
}
