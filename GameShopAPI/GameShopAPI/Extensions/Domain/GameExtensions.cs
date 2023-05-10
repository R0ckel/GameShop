using GameShopAPI.DTOs.Game;
using GameShopAPI.Models.Domain;

namespace GameShopAPI.Extensions.Domain;

public static class GameExtensions
{
    public static Game ToModel(this CreateGameRequest request)
    {
        return new Game
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            CompanyId = request.CompanyId
        };
    }

    public static Game ToModel(this UpdateGameRequest request)
    {
        var game = new Game
        {
            Id = request.Id,
            CompanyId = request.CompanyId,
            Genres = request.GenreIds.Select(id => new GameGenre { Id = id }).ToList()
        };

        if (request.Name != null)
            game.Name = request.Name;

        if (request.Price != null)
            game.Price = request.Price.Value;

        if (request.Description != null)
            game.Description = request.Description;

        return game;
    }

    public static GameResponse ToResponse(this Game game)
        => new()
        {
            Id = game.Id,
            Name = game.Name ?? string.Empty,
            Price = game.Price,
            Description = game.Description ?? string.Empty,
            CompanyId = game.CompanyId,
            CompanyName = game.Company?.Name ?? string.Empty,
            Genres = game.Genres.Select(gg => gg.ToCard()).ToList()
        };
}