using GameShopAPI.Data;
using GameShopAPI.DTOs.Comment;
using GameShopAPI.Extensions.Domain;
using GameShopAPI.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Services.CommentService;

public class CommentService : ICommentService
{
    private readonly GameShopContext _context;

    public CommentService(GameShopContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<CommentResponse>> ReadPageAsync(int page, int pageSize, CommentFilter? filter)
    {
        var response = new BaseResponse<CommentResponse>();

        try
        {
            var query = _context.Comments
                .Include(x => x.User)
                .Include(x => x.Game)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.GameId != null)
                    query = query.Where(x => x.GameId == filter.GameId);
            }

            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.ToResponse())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(comments);
            response.ValueCount = comments.Count;
            response.PageCount = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
            response.PageSize = pageSize;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CommentResponse>> CreateAsync(CreateCommentRequest request, Guid userId)
    {
        var response = new BaseResponse<CommentResponse>();

        try
        {
            var comment = request.ToModel();
            comment.UserId = userId;
            comment.Created = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status201Created;
            response.Values.Add(comment.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CommentResponse>> ReadAsync(Guid id)
    {
        var response = new BaseResponse<CommentResponse>();

        try
        {
            var comment = await _context.Comments
                .Where(x => x.Id == id)
                .Include(x => x.User)
                .Include(x => x.Game)
                .SingleAsync();

            if (comment == null)
            {
                response.Message = "Comment not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(comment.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CommentResponse>> UpdateAsync(Guid id, UpdateCommentRequest request, Guid userId)
    {
        var response = new BaseResponse<CommentResponse>();

        try
        {
            if (id != request.Id)
            {
                response.Message = "Ids don`t match";
                return response;
            }

            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                response.Message = "Comment not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (comment.UserId != userId)
            {
                response.Message = "Forbidden to change comment of another account";
                response.StatusCode = StatusCodes.Status403Forbidden;
                return response;
            }

            comment.Text = request.Text;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(comment.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CommentResponse>> DeleteAsync(Guid id, Guid userId)
    {
        var response = new BaseResponse<CommentResponse>();

        try
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                response.Message = "Comment not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (comment.UserId != userId)
            {
                response.Message = "Forbidden to delete comment of another account";
                response.StatusCode = StatusCodes.Status403Forbidden;
                return response;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
