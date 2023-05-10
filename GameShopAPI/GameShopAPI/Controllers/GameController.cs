using GameShopAPI.DTOs.Game;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.GameService;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    // GET: api/v1/Game + ?params
    [HttpGet]
    public async Task<BaseResponse<GameResponse>> Get([FromQuery] GameFilter? filter,
                                                      int page = 1,
                                                      int pageSize = 10)
        => await _gameService.ReadPageAsync(page, pageSize, filter);

    // GET api/v1/Game/5
    [HttpGet("{id}")]
    public async Task<BaseResponse<GameResponse>> Get(Guid id)
        => await _gameService.ReadAsync(id);

    // POST api/v1/Game
    [HttpPost]
    public async Task<BaseResponse<GameResponse>> Post([FromBody] CreateGameRequest request)
        => await _gameService.CreateAsync(request);

    // PUT api/v1/Game/5
    [HttpPut("{id}")]
    public async Task<BaseResponse<GameResponse>> Put(Guid id,
                                                      [FromBody] UpdateGameRequest request)
        => await _gameService.UpdateAsync(id, request);

    // DELETE api/v1/Game/5
    [HttpDelete("{id}")]
    public async Task<BaseResponse<GameResponse>> Delete(Guid id)
        => await _gameService.DeleteAsync(id);
}
