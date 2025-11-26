using Microsoft.EntityFrameworkCore;
using TakeHomeChallenge.Domain.Entities;

namespace TakeHomeChallenge.Infrastructure;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{

    //aca creo las tablas. No olvidar importar los Usings si hace falta

    public DbSet<User>? Users { get; set; }
    public DbSet<Notification>? Notifications { get; set; }
}
