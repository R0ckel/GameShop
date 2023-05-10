namespace GameShopAPI.DTOs.Comment;

public class CommentResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
