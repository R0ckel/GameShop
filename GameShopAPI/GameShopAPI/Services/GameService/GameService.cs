using GameShopAPI.Data;
using GameShopAPI.DTOs.Game;
using GameShopAPI.Extensions.Domain;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.ImageService;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Services.GameService;

public class GameService : IGameService
{
    private readonly GameShopContext _context;
    private readonly IImageService _imageService;

    public GameService(GameShopContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<BaseResponse<GameResponse>> ReadPageAsync(int page, int pageSize, GameFilter? filter)
    {
        var response = new BaseResponse<GameResponse>();

        try
        {
            var query = _context.Games
                .Include(x => x.Company)
                .Include(x => x.Genres)
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(g => g.Name != null && g.Name.Contains(filter.Name));

                if (!string.IsNullOrWhiteSpace(filter.Description))
                    query = query.Where(g => g.Description != null && g.Description.Contains(filter.Description));

                if (filter.PriceFrom != null)
                    query = query.Where(g => g.Price >= filter.PriceFrom);

                if (filter.PriceTo != null)
                    query = query.Where(g => g.Price <= filter.PriceTo);

                if (filter.CompanyName != null)
                    query = query.Where(g => g.Company != null && 
                    g.Company.Name != null &&
                    g.Company.Name.Contains(filter.CompanyName));

                if (filter.GenreIds != null && filter.GenreIds.Any())
                    query = query.Where(g => g.Genres.Any(gg => filter.GenreIds.Contains(gg.Id)));
            }

            var games = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.ToResponse())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(games);
            response.ValueCount = games.Count;
            response.PageNumber = page;
            response.PageSize = pageSize;
            response.PageCount = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<GameResponse>> ReadAsync(Guid id)
    {
        var response = new BaseResponse<GameResponse>();

        try
        {
            var game = await _context.Games
                .Where(x => x.Id == id)
                .Include(x => x.Company)
                .Include(x => x.Genres)
                .SingleAsync();

            if (game == null)
            {
                response.Message = "Game not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(game.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<GameResponse>> CreateAsync(CreateGameRequest request)
    {
        var response = new BaseResponse<GameResponse>();

        try
        {
            if (await _context.Games.AnyAsync(g => g.Name == request.Name))
            {
                response.Message = "There is a game with this name already";
                return response;
            }

            if (!await _context.Companies.AnyAsync(x => x.Id == request.CompanyId))
            {
                response.Message = "No such company";
                return response;
            }

            if (!request.GenreIds.All(g => _context.GameGenres.Any(x => x.Id == g)))
            {
                response.Message = "At least one of genres doesn`t exist in database";
                return response;
            }

            var genres = _context.GameGenres.Where(g => request.GenreIds.Contains(g.Id)).ToList();
            var game = request.ToModel();
            game.Genres = genres;

            var entry = await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            var value = entry.Entity.ToResponse();
            value.CompanyName = (await _context.Companies.FirstOrDefaultAsync(x => x.Id == value.CompanyId))?.Name ?? string.Empty;

            response.Success = true;
            response.Message = "Game created successfully";
            response.StatusCode = StatusCodes.Status201Created;
            response.Values.Add(value);
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<GameResponse>> UpdateAsync(Guid id, UpdateGameRequest request)
    {
        var response = new BaseResponse<GameResponse>();

        try
        {
            if (id != request.Id)
            {
                response.Message = "Ids don`t match";
                return response;
            }

            if (!await _context.Games.AnyAsync(g => g.Id == request.Id))
            {
                response.Message = "Game not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (await _context.Games.AnyAsync(g => g.Name == request.Name && g.Id != request.Id))
            {
                response.Message = "There is a game with this name already";
                return response;
            }

            var game = request.ToModel();
            _context.Entry(game).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Game updated successfully";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(game.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<GameResponse>> DeleteAsync(Guid id)
    {
        var response = new BaseResponse<GameResponse>();

        try
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                response.Message = "Game not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            // Delete images
            if (!string.IsNullOrWhiteSpace(game.ImagePath))
                _imageService.DeleteImage(game.ImagePath);
            if (!string.IsNullOrEmpty(game.ThumbnailImagePath))
                _imageService.DeleteImage(game.ThumbnailImagePath);

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Game deleted successfully";
            response.StatusCode = StatusCodes.Status204NoContent;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
