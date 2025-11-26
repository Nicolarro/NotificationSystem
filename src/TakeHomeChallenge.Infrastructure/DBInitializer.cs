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
                Description= "Usuario Prueba1",
                NotificationID=1

            },
            new ()
            {
                Name="Maria",
                Description= "Usuario Prueba2",
                NotificationID=1

            },
            new ()
            {
                Name="Carlos",
                Description= "Usuario Prueba3",
                NotificationID=1

            }
    };

        context.Users.AddRange(users);

        await context.SaveChangesAsync();

    }
}
