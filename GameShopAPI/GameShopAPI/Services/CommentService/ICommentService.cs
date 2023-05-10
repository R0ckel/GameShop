using GameShopAPI.DTOs.Comment;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.CommentService;

public interface ICommentService
{
    Task<BaseResponse<CommentResponse>> CreateAsync(CreateCommentRequest request, Guid userId);
    Task<BaseResponse<CommentResponse>> ReadAsync(Guid id);
    Task<BaseResponse<CommentResponse>> UpdateAsync(Guid id, UpdateCommentRequest request, Guid userId);
    Task<BaseResponse<CommentResponse>> DeleteAsync(Guid id, Guid userId);
    Task<BaseResponse<CommentResponse>> ReadPageAsync(int page, int pageSize, CommentFilter? filter);
}
