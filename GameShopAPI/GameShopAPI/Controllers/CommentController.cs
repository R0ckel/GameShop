using GameShopAPI.DTOs.Comment;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.CommentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // GET: api/v1/Comment + ?params
    [HttpGet, AllowAnonymous]
    public async Task<BaseResponse<CommentResponse>> Get([FromQuery] CommentFilter? filter,
                                                         int page = 1,
                                                         int pageSize = 10)
        => await _commentService.ReadPageAsync(page, pageSize, filter);

    // GET api/v1/Comment/5
    [HttpGet("{id}"), AllowAnonymous]
    public async Task<BaseResponse<CommentResponse>> Get(Guid id)
        => await _commentService.ReadAsync(id);

    // POST api/v1/Comment
    [HttpPost]
    public async Task<BaseResponse<CommentResponse>> Post([FromBody] CreateCommentRequest request)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<CommentResponse>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.CreateAsync(request, userId);
    }

    // PUT api/v1/Comment/5
    [HttpPut("{id}")]
    public async Task<BaseResponse<CommentResponse>> Put(Guid id,
                                                         [FromBody] UpdateCommentRequest request)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<CommentResponse>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.UpdateAsync(id, request, userId);
    }

    // DELETE api/v1/Comment/5
    [HttpDelete("{id}")]
    public async Task<BaseResponse<CommentResponse>> Delete(Guid id)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null)
        {
            return new BaseResponse<CommentResponse>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "User is not defined"
            };
        }

        var userId = Guid.Parse(user);
        return await _commentService.DeleteAsync(id, userId);
    }
}
