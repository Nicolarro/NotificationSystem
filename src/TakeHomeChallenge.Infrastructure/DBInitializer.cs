using System.Collections.ObjectModel;
using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Infrastructure;

public static class DBInitializer
{
    public static async Task SeedData(AppDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var users = new List<User>
        {
            new()
            {
                Name = "Nicolas",
                Email = "nicolarro20@gmail.com",
                Password = "default123",
                PokemonsIds = new Collection<int> { 1, 25, 150 }
            },
            new()
            {
                Name = "Toto",
                Email = "toto@gmail.com",
                Password = "default123",
                PokemonsIds = new Collection<int> { 4, 6 }
            },
            new()
            {
                Name = "Carlos",
                Email = "usuario3@gmail.com",
                Password = "default123",
                PokemonsIds = new Collection<int> { 7, 9, 131 }
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
