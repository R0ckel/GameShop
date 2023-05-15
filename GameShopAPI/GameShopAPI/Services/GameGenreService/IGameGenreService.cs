using GameShopAPI.DTOs.GameGenre;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.GameGenreService;

public interface IGameGenreService
{
    Task<BaseResponse<GameGenreResponse>> CreateAsync(CreateGameGenreRequest request);
    Task<BaseResponse<GameGenreResponse>> ReadAsync(Guid id);
    Task<BaseResponse<GameGenreResponse>> UpdateAsync(Guid id, UpdateGameGenreRequest request);
    Task<BaseResponse<GameGenreResponse>> DeleteAsync(Guid id);
    Task<BaseResponse<GameGenreResponse>> ReadPageAsync(int page, int pageSize, GameGenreFilter? filter);
    Task<BaseResponse<GameGenreCard>> ReadCardsAsync();
}