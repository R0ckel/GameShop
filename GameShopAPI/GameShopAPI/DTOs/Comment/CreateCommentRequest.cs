using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.Comment;

public class CreateCommentRequest
{
    [Required]
    public Guid GameId { get; set; }

    [Required, MaxLength(3000)]
    public string Text { get; set; } = string.Empty;
}
