using GameShopAPI.Data;
using GameShopAPI.Extensions.Domain;
using GameShopAPI.Models.Base;
using Microsoft.EntityFrameworkCore;
using GameShopAPI.DTOs.User;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using GameShopAPI.Models.Domain;
using GameShopAPI.Services.PasswordHasher;

namespace GameShopAPI.Services.UserService;

public class UserService : IUserService
{
    private readonly GameShopContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(GameShopContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<UserResponse>> CreateAsync(RegisterUserRequest request)
    {
        var response = new BaseResponse<UserResponse>();

        try
        {
            if (await _context.Users.AnyAsync(c => c.Email == request.Email))
            {
                response.Message = "This email is already taken";
                return response;
            }

            if (!IsValidPassword(request.Password))
            {
                response.Message = "Password invalid";
                return response;
            }

            var user = request.ToModel();
            _passwordHasher.SetUserPasswordHash(user, request.Password);

            EntityEntry<User>? entry = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "User created successfully";
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

    private bool IsValidPassword(string password)
    {
        if (password == null || password.Length < 8 || password.Length > 64)
            return false;

        if (!password.Any(char.IsUpper))
            return false;

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return false;

        return true;
    }

    public async Task<BaseResponse<UserResponse>> ReadAsync(Guid id)
    {
        var response = new BaseResponse<UserResponse>();

        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                response.Message = "User not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(user.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var response = new BaseResponse<UserResponse>();

        try
        {
            if (id != request.Id)
            {
                response.Message = "Ids don`t match";
                return response;
            }

            var user = await _context.Users.FindAsync(request.Id);
            if (user == null)
            {
                response.Message = "User not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (!_passwordHasher.VerifyPasswordHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "Old password don`t match";
                return response;
            }

            if (await _context.Users.AnyAsync(c => c.Email == request.Email && c.Id != request.Id))
            {
                response.Message = "There is a user with this email already";
                return response;
            }
            
            // Disable the trigger
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Users DISABLE TRIGGER SetUserLocked");

            // update user data
            if (!string.IsNullOrWhiteSpace(request.Name)) 
                user.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Surname)) 
                user.Surname = request.Surname;

            if (!string.IsNullOrWhiteSpace(request.Email)) 
                user.Email = request.Email;

            if (request.BirthDate != null) 
                user.BirthDate = request.BirthDate.Value.ToDateTime(TimeOnly.MinValue);

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
                _passwordHasher.SetUserPasswordHash(user, request.NewPassword);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Re-enable the trigger
            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Users ENABLE TRIGGER SetUserLocked");

            response.Success = true;
            response.Message = "User updated successfully";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(user.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
