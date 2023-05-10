using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.Comment;

public class UpdateCommentRequest
{
    public Guid Id { get; set; }

    [Required, MaxLength(3000)]
    public string Text { get; set; } = string.Empty;
}
