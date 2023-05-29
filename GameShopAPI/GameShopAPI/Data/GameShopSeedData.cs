using GameShopAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Data;

public static class GameShopSeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        using var context = new GameShopContext(configuration);

        if (context.Games.Any())
        {
            return;   // DB has been seeded
        }

        // Create some companies
        var company1 = new Company { Name = "Nintendo", Country = "Japan" };
        var company2 = new Company { Name = "Valve Corporation", Country = "USA" };
        var company3 = new Company { Name = "Rockstar Games", Country = "USA" };
        var company4 = new Company { Name = "Electronic Arts", Country = "USA" };
        var company5 = new Company { Name = "Ubisoft", Country = "France" };
        var company6 = new Company { Name = "Activision Blizzard", Country = "USA" };

        // Add companies to the database
        context.Companies.AddRange(company1, company2, company3, company4, company5, company6);

        // Create some game genres
        var genre1 = new GameGenre { Name = "Action", Description = "Action games emphasize physical challenges that require hand-eye coordination and motor skill to overcome." };
        var genre2 = new GameGenre { Name = "Adventure", Description = "Adventure games focus on puzzle solving within a narrative framework." };
        var genre3 = new GameGenre { Name = "Role-playing", Description = "Role-playing games involve players taking on the roles of characters in a fictional setting." };
        var genre4 = new GameGenre { Name = "Strategy", Description = "Strategy games focus on gameplay requiring careful and skillful thinking and planning in order to achieve victory." };
        var genre5 = new GameGenre { Name = "Simulation", Description = "Simulation games involve the player controlling real-world vehicles or characters in a realistic setting." };
        var genre6 = new GameGenre { Name = "Sports", Description = "Sports games simulate the playing of traditional physical sports." };
        var genre7 = new GameGenre { Name = "Racing", Description = "Racing games involve the player competing in races with vehicles or characters." };

        // Add game genres to the database
        context.GameGenres.AddRange(genre1, genre2, genre3, genre4, genre5, genre6, genre7);

        // Create some games
        var game1 = new Game { Name = "The Legend of Zelda: Breath of the Wild", Price = 59.99, Description = "An action-adventure game set in a large open world environment.", CompanyId = company1.Id };
        game1.Genres.Add(genre1);
        game1.Genres.Add(genre2);

        var game2 = new Game { Name = "Half-Life 2", Price = 9.99, Description = "A first-person shooter game set in a dystopian world.", CompanyId = company2.Id };
        game2.Genres.Add(genre1);

        var game3 = new Game { Name = "Red Dead Redemption 2", Price = 59.99, Description = "An action-adventure game set in the American Old West.", CompanyId = company3.Id };
        game3.Genres.Add(genre1);
        game3.Genres.Add(genre2);

        var game4 = new Game { Name = "The Sims 4", Price = 39.99, Description = "A life simulation game where players create and control virtual people.", CompanyId = company4.Id };
        game4.Genres.Add(genre5);

        var game5 = new Game { Name = "Assassin's Creed Valhalla", Price = 59.99, Description = "An action role-playing game set in the Viking Age.", CompanyId = company5.Id };
        game5.Genres.Add(genre1);
        game5.Genres.Add(genre3);

        var game6 = new Game { Name = "Call of Duty: Modern Warfare", Price = 59.99, Description = "A first-person shooter game set in a modern-day conflict.", CompanyId = company6.Id };
        game6.Genres.Add(genre1);

        // Add games to the database
        context.Games.AddRange(game1, game2, game3, game4, game5, game6);

        // Save changes to the database
        context.SaveChanges();
    }
}
