using GameShopAPI.Models.Base;
using GameShopAPI.Models.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameShopAPI.DTOs.User;
using GameShopAPI.Services.PasswordHasher;
using GameShopAPI.Data;
using Microsoft.EntityFrameworkCore;
using Azure;

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

    public async Task<BaseResponse<AuthResponse>> Login(LoginUserRequest request, HttpResponse response)
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
            var tokenParts = token.Split('.');
            string headerAndPayload = tokenParts[0] + '.' + tokenParts[1];
            string signature = tokenParts[2];

            // add auth info in 2 separate cookies
            // 1 with info (payload), and 1 with signature as userkey (+ secure)
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddHours(6),
                IsEssential = true,
            };
            response.Cookies.Append("headerAndPayload", headerAndPayload, cookieOptions);
            var signatureCookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddHours(6),
                IsEssential = true,
            };
            response.Cookies.Append("signature", signature, signatureCookieOptions);

            var role = await _context.UserRoles.FirstOrDefaultAsync(x => x.Id == user.RoleId);
            var authResponse = new AuthResponse
            {
                UserName = $"{user.Name} {user.Surname}",
                Role = role?.Name ?? string.Empty,
            };

            return new BaseResponse<AuthResponse>
            {
                Success = true,
                Message = "Success",
                StatusCode = StatusCodes.Status200OK,
                ValueCount = 1,
                Values = new List<AuthResponse> { authResponse }
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

    public BaseResponse<bool> Logout(HttpResponse response)
    {
        try
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
            };
            var signatureCookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
            };
            response.Cookies.Delete("headerAndPayload", cookieOptions);
            response.Cookies.Delete("signature", signatureCookieOptions);

            return new BaseResponse<bool>
            {
                Message = "Logout successful",
                Success = true,
                StatusCode = StatusCodes.Status200OK,
            };
        }
        catch (Exception ex)
        {
            // Handle the exception here
            return new BaseResponse<bool>()
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
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("Authorization:TokenKey").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(6),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
