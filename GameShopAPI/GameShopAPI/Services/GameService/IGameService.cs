using GameShopAPI.DTOs.Game;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.GameService;

public interface IGameService
{
    Task<BaseResponse<GameResponse>> CreateAsync(CreateGameRequest request);
    Task<BaseResponse<GameResponse>> ReadAsync(Guid id);
    Task<BaseResponse<GameResponse>> UpdateAsync(Guid id, UpdateGameRequest request);
    Task<BaseResponse<GameResponse>> DeleteAsync(Guid id);
    Task<BaseResponse<GameResponse>> ReadPageAsync(int page, int pageSize, GameFilter? filter);
}
