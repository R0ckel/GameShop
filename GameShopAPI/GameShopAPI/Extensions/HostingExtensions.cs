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
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

    public static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameShopAPI", Version = "v1" });
            c.AddSecurityDefinition(
                "token",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Name = HeaderNames.Authorization
                }
            );
            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "token"
                        },
                    },
                    Array.Empty<string>()
                }
                }
            );
        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration.GetSection("Authorization:TokenKey").Value!)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return builder;
    }

    public static IApplicationBuilder UseSeedData(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            SeedData.Initialize(services);
        }

        return app;
    }
}
