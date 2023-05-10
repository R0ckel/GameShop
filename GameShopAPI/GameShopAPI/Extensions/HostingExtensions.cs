using GameShopAPI.Data;
using GameShopAPI.DTOs.Game;
using GameShopAPI.Services.AuthService;
using GameShopAPI.Services.CompanyService;
using GameShopAPI.Services.GameGenreService;
using GameShopAPI.Services.GameImagesService;
using GameShopAPI.Services.GameService;
using GameShopAPI.Services.IModelImageService;
using GameShopAPI.Services.ImageService;
using GameShopAPI.Services.PasswordHasher;
using GameShopAPI.Services.UserRoleService;
using GameShopAPI.Services.UserService;
using GameShopAPI.Services.UserImageService;
using GameShopAPI.DTOs.User;
using GameShopAPI.Services.CommentService;
using GameShopAPI.Services.BasketService;

namespace GameShopAPI.Extensions;

public static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IImageService, ImageService>();

        builder.Services.AddScoped<ICompanyService, CompanyService>();
        builder.Services.AddScoped<IGameGenreService, GameGenreService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IUserRoleService, UserRoleService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IBasketService, BasketService>();

        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddScoped<IModelImageService<GameResponse>, GameImageService>();
        builder.Services.AddScoped<IModelImageService<UserResponse>, UserImageService>();

        return builder;
    }

    public static WebApplicationBuilder ConfigureContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<GameShopContext>();

        return builder;
    }
}
