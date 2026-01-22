using System.Data.Common;
using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Infrastructure;

public class DBInitializer
{

    public static async Task SeedData(AppDbContext context)

    {
        if (context.Users?.Any() ?? false) return;

        var users = new List<User>()
        {
            new ()
            {
                Name="Nicolas",
                Email = "nicolarro20@gmail.com",

            },
            new ()
            {
                Name="Toto",
                Email = "toto@gmail.com",
    

            },
            new ()
            {
                Name="Carlos",
                Email= "usuario3@gmail.com",

            }
    };

        context.Users.AddRange(users);

        await context.SaveChangesAsync();

    }
}
