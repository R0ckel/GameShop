using GameShopAPI.DTOs.GameGenre;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.GameGenreService;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameGenreController : ControllerBase
{
    private IGameGenreService _gameGenreService;

    public GameGenreController(IGameGenreService gameGenreService)
    {
        _gameGenreService = gameGenreService;
    }

    // GET: api/v1/GameGenre + ?params
    [HttpGet]
    public async Task<BaseResponse<GameGenreResponse>> Get([FromQuery] GameGenreFilter? filter,
                                                           int page = 1,
                                                           int pageSize = 10)
        => await _gameGenreService.ReadPageAsync(page, pageSize, filter);

    // GET api/v1/GameGenre/5
    [HttpGet("{id}")]
    public async Task<BaseResponse<GameGenreResponse>> Get(Guid id)
        => await _gameGenreService.ReadAsync(id);

    // POST api/v1/GameGenre
    [HttpPost]
    public async Task<BaseResponse<GameGenreResponse>> Post([FromBody] CreateGameGenreRequest request)
        => await _gameGenreService.CreateAsync(request);

    // PUT api/v1/GameGenre/5
    [HttpPut("{id}")]
    public async Task<BaseResponse<GameGenreResponse>> Put(Guid id,
                                                           [FromBody] UpdateGameGenreRequest request)
        => await _gameGenreService.UpdateAsync(id, request);

    // DELETE api/v1/GameGenre/5
    [HttpDelete("{id}")]
    public async Task<BaseResponse<GameGenreResponse>> Delete(Guid id)
        => await _gameGenreService.DeleteAsync(id);
}
