using GameShopAPI.Data;
using GameShopAPI.DTOs.GameGenre;
using GameShopAPI.Extensions.Domain;
using GameShopAPI.Models.Base;
using GameShopAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Services.GameGenreService;

public class GameGenreService : IGameGenreService
{
    private readonly GameShopContext _context;

    public GameGenreService(GameShopContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<GameGenreResponse>> ReadPageAsync(int page, int pageSize, GameGenreFilter? filter)
    {
        var response = new BaseResponse<GameGenreResponse>();

        try
        {
            var query = _context.GameGenres.AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(gg => gg.Name != null && gg.Name.Contains(filter.Name));

                if (!string.IsNullOrWhiteSpace(filter.Description))
                    query = query.Where(gg => gg.Description != null && gg.Description.Contains(filter.Description));
            }

            var gameGenres = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.ToResponse())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(gameGenres);
            response.ValueCount = gameGenres.Count;
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

    public async Task<BaseResponse<GameGenreCard>> ReadCardsAsync()
    {
        var response = new BaseResponse<GameGenreCard>();

        try
        {
            var genres = await _context.GameGenres
                .Select(x => x.ToCard())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(genres);
            response.ValueCount = genres.Count;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<GameGenreResponse>> ReadAsync(Guid id)
    {
        var response = new BaseResponse<GameGenreResponse>();

        try
        {
            var gameGenre = await _context.GameGenres.FindAsync(id);

            if (gameGenre == null)
            {
                response.Message = "Game genre not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(gameGenre.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<GameGenreResponse>> CreateAsync(CreateGameGenreRequest request)
    {
        var response = new BaseResponse<GameGenreResponse>();

        try
        {
            if (await _context.GameGenres.AnyAsync(gg => gg.Name == request.Name))
            {
                response.Message = "There is a game genre with this name already";
                return response;
            }

            var entry = await _context.GameGenres.AddAsync(request.ToModel());
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Game genre created successfully";
            response.StatusCode = StatusCodes.Status201Created;
            response.Values.Add(entry.Entity.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
    public async Task<BaseResponse<GameGenreResponse>> UpdateAsync(Guid id, UpdateGameGenreRequest request)
    {
        var response = new BaseResponse<GameGenreResponse>();

        try
        {
            if (id != request.Id)
            {
                response.Message = "Ids don`t match";
                return response;
            }

            if (!await _context.GameGenres.AnyAsync(gg => gg.Id == request.Id))
            {
                response.Message = "Game genre not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }
            if (await _context.GameGenres.AnyAsync(gg => gg.Name == request.Name && gg.Id != request.Id))
            {
                response.Message = "There is a game genre with this name already";
                return response;
            }

            var gameGenre = request.ToModel();
            _context.Entry(gameGenre).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Game genre updated successfully";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(gameGenre.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<GameGenreResponse>> DeleteAsync(Guid id)
    {
        var response = new BaseResponse<GameGenreResponse>();

        try
        {
            var gameGenre = await _context.GameGenres.FindAsync(id);

            if (gameGenre == null)
            {
                response.Message = "Game genre not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            _context.GameGenres.Remove(gameGenre);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Game genre deleted successfully";
            response.StatusCode = StatusCodes.Status204NoContent;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
