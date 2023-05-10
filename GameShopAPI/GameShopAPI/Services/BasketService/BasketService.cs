using GameShopAPI.Data;
using GameShopAPI.DTOs.Basket;
using GameShopAPI.Extensions.Domain;
using GameShopAPI.Models.Base;
using GameShopAPI.Models.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GameShopAPI.Services.BasketService;

public class BasketService : IBasketService
{
    private readonly GameShopContext _context;

    public BasketService(GameShopContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<BasketItemResponse>> AddAsync(AddBasketItemRequest request, Guid userId)
    {
        var response = new BaseResponse<BasketItemResponse>();

        try
        {
            if (!_context.Users.Any(x => x.Id == userId))
            {
                response.Message = "User not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (!_context.Games.Any(x => x.Id == request.GameId))
            {
                response.Message = "Game not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (_context.BasketItems.Any(x => x.GameId == request.GameId && x.UserId == userId))
            {
                response.Message = "Already in basket";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

            var basketItem = new BasketItem
            {
                UserId = userId,
                GameId = request.GameId
            };

            var entry = _context.BasketItems.Add(basketItem);
            await _context.SaveChangesAsync();

            var game = await _context.Games.FindAsync(request.GameId);
            entry.Entity.Game = game;

            response.Success = true;
            response.Message = "Added successfully";
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

    public async Task<BaseResponse<BasketItemResponse>> RemoveAsync(RemoveBasketItemRequest request, Guid userId)
    {
        var response = new BaseResponse<BasketItemResponse>();

        try
        {
            var basketItem = await _context.BasketItems.FindAsync(userId, request.GameId);

            if (basketItem == null)
            {
                response.Message = "Basket item not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            _context.BasketItems.Remove(basketItem);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<BasketItemResponse>> ReadAsync(Guid userId)
    {
        var response = new BaseResponse<BasketItemResponse>();

        try
        {
            var basketItems = await _context.BasketItems
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Game)
                .Select(x => x.ToResponse())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(basketItems);
            response.ValueCount = basketItems.Count;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<BasketItemResponse>> RemoveAllAsync(Guid userId)
    {
        var response = new BaseResponse<BasketItemResponse>();

        try
        {
            // using store procedure to clear basket

            var resultParam = new SqlParameter("@result", SqlDbType.Int) { Direction = ParameterDirection.Output };
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC ClearBasket @userId, @result OUTPUT",
                new SqlParameter("@userId", userId), resultParam);
            int result = (int)resultParam.Value;

            if (result != 0)
            {
                response.Message = "SQL Error #" + result;
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

            response.Success = true;
            response.Message = "Basket cleared successfully";
            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<decimal>> GetBasketTotalPrice(Guid userId)
    {
        var response = new BaseResponse<decimal>();

        try
        {
            // using db function to calculate basket total

            var basketTotal = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => GameShopContext.GetBasketTotal(u.Id))
                .FirstOrDefaultAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(basketTotal ?? 0);
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
