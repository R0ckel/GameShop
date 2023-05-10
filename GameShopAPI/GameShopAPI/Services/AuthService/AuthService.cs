using GameShopAPI.Models.Base;
using GameShopAPI.Models.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameShopAPI.DTOs.User;
using GameShopAPI.Services.PasswordHasher;
using GameShopAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly GameShopContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IConfiguration configuration, GameShopContext context, IPasswordHasher passwordHasher)
    {
        _configuration = configuration;
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<AuthResponse>> Login(LoginUserRequest request)
    {
        try
        {
            var authFailedResult = new BaseResponse<AuthResponse>
            {
                Message = "Failed to login",
                ValueCount = 1,
                Values = new List<AuthResponse> { new AuthResponse() { Message = "Authentication failed" } }
            };

            var user = await _context
                .Users
                .Where(x => x.Email != null && 
                                x.Email.Equals(request.Email))
                .Where(x => x.IsLocked == false)
                .Include(x => x.Role)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return authFailedResult;
            }

            if (!_passwordHasher.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Users SET AuthFailedCount = AuthFailedCount + 1 WHERE Id = {0}", user.Id);
                return authFailedResult;
            }

            string token = CreateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Success = true,
                UserName = $"{user.Name} {user.Surname}"
            };

            return new BaseResponse<AuthResponse>
            {
                Message = response.Success ? "Success" : "Login error",
                StatusCode = response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                ValueCount = 1,
                Values = new List<AuthResponse> { response }
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<AuthResponse>()
            {
                Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message
            };
        }
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("Authorization:TokenKey").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(3),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
