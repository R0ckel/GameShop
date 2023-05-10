using GameShopAPI.DTOs.Comment;
using GameShopAPI.Models.Domain;

namespace GameShopAPI.Extensions.Domain;

public static class CommentExtensions
{
    public static Comment ToModel(this CreateCommentRequest request)
    {
        return new Comment
        {
            GameId = request.GameId,
            Text = request.Text
        };
    }

    public static Comment ToModel(this UpdateCommentRequest request)
    {
        return new Comment
        {
            Id = request.Id,
            Text = request.Text
        };
    }

    public static CommentResponse ToResponse(this Comment comment)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            GameId = comment.GameId,
            GameName = comment.Game?.Name ?? string.Empty,
            UserId = comment.UserId,
            UserName = $"{comment.User?.Name} {comment.User?.Surname}",
            Text = comment.Text ?? string.Empty
        };
    }
}
