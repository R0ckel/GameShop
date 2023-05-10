using GameShopAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;
namespace GameShopAPI.Data;

public class GameShopContext : DbContext
{
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameGenre> GameGenres => Set<GameGenre>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();

    private readonly IConfiguration _configuration;

    public GameShopContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("base"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //configure Basket table
        modelBuilder.Entity<BasketItem>()
            .HasKey(bi => new { bi.UserId, bi.GameId });

        modelBuilder.Entity<BasketItem>()
            .HasOne(bi => bi.User)
            .WithMany(u => u.BasketItems)
            .HasForeignKey(bi => bi.UserId);

        modelBuilder.Entity<BasketItem>()
            .HasOne(bi => bi.Game)
            .WithMany(g => g.BasketItems)
            .HasForeignKey(bi => bi.GameId);

        // create indexes
        modelBuilder.Entity<BasketItem>()
            .HasIndex(bi => new { bi.UserId, bi.GameId })
            .IsUnique();

        modelBuilder.Entity<Comment>()
            .HasIndex(c => new { c.UserId, c.GameId, c.Id })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }

    [DbFunction("GetBasketTotal", "dbo")]
    public static decimal? GetBasketTotal(Guid userId)
    {
        throw new NotSupportedException();

        // this code shows to App, that there is this function, and can be used in LINQ, but can`t be used elsewhere

        // code snippet

        // using (var context = new GameShopContext())
        // {
        //    var userId = new Guid("...");
        //    var basketTotal = context.Users
        //        .Where(u => u.Id == userId)
        //        .Select(u => GameShopContext.GetBasketTotal(u.Id))
        //        .Single();
        // }
    }
}
