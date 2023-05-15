using GameShopAPI.Data;
using GameShopAPI.DTOs.UserRole;
using GameShopAPI.Extensions.Domain;
using GameShopAPI.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Services.UserRoleService;

public class UserRoleService : IUserRoleService
{
    private readonly GameShopContext _context;

    public UserRoleService(GameShopContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<UserRoleResponse>> ReadPageAsync(int page, int pageSize, UserRoleFilter? filter)
    {
        var response = new BaseResponse<UserRoleResponse>();

        try
        {
            var query = _context.UserRoles.AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(c => c.Name != null && c.Name.Contains(filter.Name));

                if (!string.IsNullOrWhiteSpace(filter.Description))
                    query = query.Where(c => c.Description != null && c.Description.Contains(filter.Description));
            }

            var userRoles = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.ToResponse())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(userRoles);
            response.ValueCount = userRoles.Count;
            response.PageNumber = page;
            response.PageSize = pageSize;
            response.PageCount = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<UserRoleResponse>> ReadAsync(int id)
    {
        var response = new BaseResponse<UserRoleResponse>();

        try
        {
            var userRole = await _context.UserRoles.FindAsync(id);

            if (userRole == null)
            {
                response.Message = "User role not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(userRole.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<UserRoleResponse>> CreateAsync(CreateUserRoleRequest request)
    {
        var response = new BaseResponse<UserRoleResponse>();

        try
        {
            if (await _context.UserRoles.AnyAsync(c => c.Name == request.Name))
            {
                response.Message = "There is a user role with this name already";
                return response;
            }

            var entry = await _context.UserRoles.AddAsync(request.ToModel());
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "User role created successfully";
            response.StatusCode = StatusCodes.Status201Created;
            response.Values.Add(entry.Entity.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<UserRoleResponse>> UpdateAsync(int id, UpdateUserRoleRequest request)
    {
        var response = new BaseResponse<UserRoleResponse>();

        try
        {
            if (id != request.Id)
            {
                response.Message = "Ids don`t match";
                return response;
            }

            if (!await _context.UserRoles.AnyAsync(c => c.Id == request.Id))
            {
                response.Message = "UserRole not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (await _context.UserRoles.AnyAsync(c => c.Name == request.Name && c.Id != request.Id))
            {
                response.Message = "There is a userRole with this name already";
                return response;
            }

            var userRole = request.ToModel();
            _context.Entry(userRole).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "UserRole updated successfully";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(userRole.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<UserRoleResponse>> DeleteAsync(int id)
    {
        var response = new BaseResponse<UserRoleResponse>();

        try
        {
            var userRole = await _context.UserRoles.FindAsync(id);

            if (userRole == null)
            {
                response.Message = "UserRole not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "UserRole deleted successfully";
            response.StatusCode = StatusCodes.Status204NoContent;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
